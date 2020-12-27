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
	Player[] allPlayers;
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
		
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{

	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		
	}

	void Start()
    {
		if(PhotonNetwork.IsConnected)
			roomCodeText.text = PhotonNetwork.CurrentRoom.Name;
		SpawnPlayer();
	}

	void SpawnPlayer()
	{
		StartCoroutine(SpawnPlayerNow());
	}

	IEnumerator SpawnPlayerNow()
	{
		yield return new WaitForSeconds(0.5f);
		id = PhotonNetwork.LocalPlayer.GetPlayerNumber();
		object[] objecttype = new object[1];
		objecttype[0] = id;
		var playerObject = PhotonNetwork.Instantiate(playerPrefab.name, SpawnPoints[id].position, SpawnPoints[id].rotation, 0, objecttype);
		for(int i = 1; i < 8; i++)
			SpawnBots(i);
	}

	void SpawnBots(int i)
	{
		int characterNumber = Random.Range(0, 6);
		int skinNumber = Random.Range(0, 3);
		object[] objecttype = new object[4];
		id = i;
		objecttype[0] = id;
		objecttype[1] = "Bot";
		objecttype[2] = characterNumber;
		objecttype[3] = skinNumber;
		var playerObject = PhotonNetwork.InstantiateSceneObject(playerPrefab.name, SpawnPoints[id].position, SpawnPoints[id].rotation, 0, objecttype);
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
