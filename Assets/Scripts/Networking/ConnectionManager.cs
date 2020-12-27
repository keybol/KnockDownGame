using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public enum ConnectionMode { Create, Join, Random }

public class ConnectionManager : MonoBehaviourPunCallbacks
{
#if UNITY_WEBGL
	[DllImport("__Internal")]
	private static extern void OpenExternalLink(string url);
#endif
	[SerializeField] private string gameVersion;
	[SerializeField] byte maxPlayersPerRoom = 8;
	[SerializeField] private TextMeshProUGUI statusText;
	[SerializeField] private TextMeshProUGUI versionText;
	[SerializeField] private TextMeshProUGUI ccuText;
	[SerializeField] private Button[] skinButtons;
	[SerializeField] private Button[] serverButtons;
	private bool tryConnecting;
	private bool isConnecting;
	private int randomRoomNumber;
	private string joinCode = "";
	private ConnectionMode connectionMode = ConnectionMode.Create;
	private ExitGames.Client.Photon.Hashtable MyPlayerProperties = new ExitGames.Client.Photon.Hashtable();

	public override void OnEnable()
	{
		base.OnEnable();
		PhotonNetwork.AddCallbackTarget(this);
	}

	public override void OnDisable()
	{
		base.OnDisable();
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	void Start()
    {
		Screen.SetResolution(960, 540, false);
		PhotonNetwork.AutomaticallySyncScene = true;
		ConnectUsingConfigs();
		ChangeServer(-1);
	}

	void Update()
    {
		versionText.text = "ver.2.5." + gameVersion;
		versionText.text += ", region: " + PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion + " Ping: " + PhotonNetwork.GetPing() + " ms";
		ccuText.text = "Online: " + PhotonNetwork.CountOfPlayersOnMaster;
	}

	public void Submit(int val)
	{
		if (!tryConnecting)
		{
			switch (val)
			{
				case 0:
					randomRoomNumber = UnityEngine.Random.Range(1000, 9999);
					joinCode = randomRoomNumber.ToString();
					connectionMode = ConnectionMode.Create;
					break;
				case 1:
					if (joinCode == "")
					{
						statusText.text = "Room Code can't be empty";
					}
					else if (joinCode.Length < 4)
					{
						statusText.text = "Room Code must be 4 digits";
						return;
					}
					connectionMode = ConnectionMode.Join;
					break;
				case 2:
					randomRoomNumber = UnityEngine.Random.Range(1000, 9999);
					joinCode = randomRoomNumber.ToString();
					connectionMode = ConnectionMode.Random;
					break;
			}
			if (joinCode == "" || joinCode.Length < 4)
				return;
			StartCoroutine(StartConnecting());
			SetCustomProperties();
			tryConnecting = true;
		}
	}

	void SetCustomProperties()
	{
		MyPlayerProperties["CharacterNumber"] = AvatarManager.Instance.characterNumber;
		MyPlayerProperties["SkinNumber"] = AvatarManager.Instance.skinNumber;
		PhotonNetwork.LocalPlayer.CustomProperties = MyPlayerProperties;
	}

	public override void OnConnectedToMaster()
	{
		JoinOrCreateRoom();
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		tryConnecting = false;
		isConnecting = false;
		if (cause.ToString() == "MaxCcuReached")
			statusText.text = "Max CCU reached. Please try again.";
		else
			statusText.text = "Network error. Please check your connection.";
	}

	void JoinOrCreateRoom()
	{
		if (isConnecting)
		{
			switch (connectionMode)
			{
				case ConnectionMode.Create:
					CreateRoom();
					statusText.text = "Room " + joinCode + " created";
					break;
				case ConnectionMode.Join:
					PhotonNetwork.JoinRoom(joinCode);
					statusText.text = "Looking for Room " + joinCode;
					break;
				case ConnectionMode.Random:
					statusText.text = "Joined Random Room";
					PhotonNetwork.JoinRandomRoom();
					break;
			}
		}
	}

	public void CreateRoom()
	{
		RoomOptions _roomOptions = new RoomOptions()
		{
			IsVisible = true,
			IsOpen = true,
			MaxPlayers = maxPlayersPerRoom
		};
		PhotonNetwork.CreateRoom(randomRoomNumber.ToString(), _roomOptions, TypedLobby.Default);
	}

	public void CodeInput(string code)
	{
		joinCode = code;
	}

	public override void OnJoinedRoom()
	{
		statusText.text = "Loading...";
		PhotonNetwork.LoadLevel("Forest");
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		switch (connectionMode)
		{
			case ConnectionMode.Create:
				break;
			case ConnectionMode.Join:
				break;
			case ConnectionMode.Random:
				CreateRoom();
				statusText.text = "No room found, creating new room " + joinCode;
				break;
		}
		isConnecting = false;
	}

	void ConnectUsingConfigs()
	{
		PhotonNetwork.ConnectUsingSettings();
		PhotonNetwork.GameVersion = gameVersion;
	}

	IEnumerator StartConnecting()
	{
		yield return new WaitForSeconds(0.25f);
		ConnectToRoom();
	}

	private void ConnectToRoom()
	{
		isConnecting = true;
		switch (connectionMode)
		{
			case ConnectionMode.Create:
				statusText.text = "Creating New Room " + joinCode;
				break;
			case ConnectionMode.Join:
				statusText.text = "Joining Room Code " + joinCode;
				break;
			case ConnectionMode.Random:
				statusText.text = "Joining Random Room";
				break;
		}
		if (!PhotonNetwork.IsConnected)
		{
			ConnectUsingConfigs();
		}
		else
		{
			JoinOrCreateRoom();
		}
	}

	public void ChangeServer(int val)
	{
		for (int i = 0; i < serverButtons.Length; i++)
			serverButtons[i].interactable = true;
		string region = "";
		switch (val)
		{
			case -1: region = ""; break;
			case 0: region = "asia"; break;
			case 1: region = "us"; break;
			case 2: region = "eu"; break;
		}
		//if (val >= 0)
		//{
		//	serverButtons[val].interactable = false;
		//	PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = region;
		//}
		//else
		//{
		//	switch (PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion)
		//	{
		//		case "asia": serverButtons[0].interactable = false; break;
		//		case "us": serverButtons[1].interactable = false; break;
		//		case "eu": serverButtons[2].interactable = false; break;
		//		default:
		//			region = "asia";
		//			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = region;
		//			break;
		//	}
		//}
		if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
		{
			PhotonNetwork.Disconnect();
			PhotonNetwork.NetworkingClient.State = ClientState.Disconnected;
			ConnectUsingConfigs();
		}
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		PhotonNetwork.Disconnect();
		switch (message)
		{
			case "Game closed":
				statusText.text = "Room already started";
				break;
			case "Game does not exist":
				statusText.text = "Room code not found";
				break;
			default:
				statusText.text = message;
				break;
		}
		tryConnecting = false;
		isConnecting = false;
	}
}