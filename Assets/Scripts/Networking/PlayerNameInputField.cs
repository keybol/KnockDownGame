// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerNameInputField.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Let the player input his name to be saved as the network player Name, viewed by alls players above each  when in the same room. 
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInputField : MonoBehaviour
{
	#region Private Constants

	// Store the PlayerPref Key to avoid typos
	[SerializeField] TMP_InputField inputMessage;
	const string playerNamePrefKey = "playerNamePrefKey";
	string defaultName = "New Player";
	GameObject canvas;

	#endregion

	#region MonoBehaviour CallBacks

	/// <summary>
	/// MonoBehaviour method called on GameObject by Unity during initialization phase.
	/// </summary>
	void Start()
	{
		SetPlayerName();
	}

	public void SetPlayerName()
	{
		defaultName = "New Player";
		TMP_InputField _inputField = this.GetComponent<TMP_InputField>();

		if (_inputField != null)
		{
			//if (PlayerPrefs.HasKey(playerNamePrefKey))
				defaultName = PlayerPrefs.GetString(playerNamePrefKey);
			//defaultName = GamePlayer.Instance.playerName;
			_inputField.text = defaultName;
		}
		PhotonNetwork.NickName = defaultName;
	}

	#endregion

	#region Public Methods

	/// <param name="value">The name of the Player</param>
	public void SetPlayerName(string namevalue)
	{
		// #Important
		if (string.IsNullOrEmpty(namevalue))
		{
			//Debug.LogError("Player Name is null or empty");
			return;
		}
		PhotonNetwork.NickName = namevalue;
		PlayerPrefs.SetString(playerNamePrefKey, namevalue);
		//GamePlayer.Instance.playerName = namevalue;
		//GamePlayer.Instance.SavePlayer();
	}

	public void SelectInputText()
	{
		inputMessage.ActivateInputField();
	}
	#endregion
}