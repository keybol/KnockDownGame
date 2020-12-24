using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
	[SerializeField] public GameObject playerPrefab;
	[SerializeField] public List<Transform> SpawnPoints;
	[SerializeField] public TextMeshProUGUI roomCodeText;
	public enum SpawnSequence { Connection, Random, RoundRobin }
	public SpawnSequence Sequence = SpawnSequence.Connection;
	protected int lastUsedSpawnPointIndex = -1;
	private int id;

	public override void OnEnable()
	{
		base.OnEnable();
		PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
	}

	public override void OnDisable()
	{
		base.OnDisable();
		PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
	}

	private void OnPlayerNumberingChanged()
	{
		id = PhotonNetwork.LocalPlayer.GetPlayerNumber();
		if (KGameManager.Instance)
			KGameManager.Instance.playerIndex = id;
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		PlayerCountUpdate();
		int playerNumber = otherPlayer.GetPlayerNumber();
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		PlayerCountUpdate();
	}

	public void PlayerCountUpdate()
	{
	}

	void Start()
    {
		PhotonNetwork.OfflineMode = true;
		CreateRoom();
		if (PhotonNetwork.IsConnected)
		{
			roomCodeText.text = PhotonNetwork.CurrentRoom.Name;
			PlayerCountUpdate();
			SpawnPlayer();
		}
	}

	public void CreateRoom()
	{
		RoomOptions _roomOptions = new RoomOptions()
		{
			IsVisible = true,
			IsOpen = true,
			MaxPlayers = 4
		};
		PhotonNetwork.CreateRoom("", _roomOptions, TypedLobby.Default);
	}

	void SpawnPlayer()
	{
		lastUsedSpawnPointIndex += PhotonNetwork.PlayerList.Length;
		StartCoroutine(SpawnPlayerNow());
	}

	IEnumerator SpawnPlayerNow()
	{
		yield return new WaitForSeconds(0.5f);
		GameObject o = playerPrefab;
		Vector3 spawnPos; Quaternion spawnRot;
		GetSpawnPoint(out spawnPos, out spawnRot);
		Vector3 newPosition = spawnPos + o.transform.position;
		object[] objecttype = new object[1];
		var playerObject = PhotonNetwork.Instantiate(o.name, newPosition, spawnRot, 0, objecttype);
	}

	protected virtual Transform GetSpawnPoint()
	{
		// Fetch a point using the Sequence method indicated
		if (SpawnPoints == null || SpawnPoints.Count == 0)
		{
			return null;
		}
		else
		{
			switch (Sequence)
			{
				case SpawnSequence.Connection:
					{
						id = PhotonNetwork.LocalPlayer.GetPlayerNumber();
						int finId = (id == -1) ? 0 : id % SpawnPoints.Count;
						KGameManager.Instance.playerIndex = finId;
						//int id = PhotonNetwork.LocalPlayer.ActorNumber - 1;
						return SpawnPoints[finId];
					}

				case SpawnSequence.RoundRobin:
					{
						lastUsedSpawnPointIndex++;
						if (lastUsedSpawnPointIndex >= SpawnPoints.Count)
							lastUsedSpawnPointIndex = 0;

						/// Use Vector.Zero and Quaternion.Identity if we are dealing with no or a null spawnpoint.
						return ReferenceEquals(SpawnPoints, null) || SpawnPoints.Count == 0 ? null : SpawnPoints[lastUsedSpawnPointIndex];
					}

				case SpawnSequence.Random:
					{
						return SpawnPoints[Random.Range(0, SpawnPoints.Count)];
					}

				default:
					return null;
			}

		}
	}

	protected virtual void GetSpawnPoint(out Vector3 spawnPos, out Quaternion spawnRot)
	{

		// Fetch a point using the Sequence method indicated
		Transform point = GetSpawnPoint();

		if (point != null)
		{
			spawnPos = point.position;
			spawnRot = point.rotation;
		}
		else
		{
			spawnPos = Vector3.zero;
			spawnRot = Quaternion.identity;
		}

		Vector3 random = Random.insideUnitSphere;
		random.y = 0;
		random = random.normalized;
	}

	IEnumerator DisconnectAndLoad()
	{
		PhotonNetwork.Disconnect();
		while (PhotonNetwork.IsConnected)
			yield return null;
		SceneManager.LoadScene("Menu");
	}

	public void DisconnectPlayer()
	{
		StartCoroutine(DisconnectAndLoad());
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		base.OnDisconnected(cause);
	}
}
