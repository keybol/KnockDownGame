// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerNameInputField.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Let the player input his name to be saved as the network player Name, viewed by alls players above each  when in the same room. 
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

/// <summary>
/// Player name input field. Let the user input his name, will appear above the player in the game.
/// </summary>
//[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{
	//[SerializeField] TextMeshProUGUI _inputField;
	[SerializeField] InputField _inputField;
	const string playerNamePrefKey = "PlayerName";

	void Start ()
	{
		int randNumber = Random.Range(99, 1000);
		string defaultName = "Basher" + randNumber;
		if (PlayerPrefs.HasKey(playerNamePrefKey))
		{
			defaultName = PlayerPrefs.GetString(playerNamePrefKey);
		}
		_inputField.text = defaultName;
		PhotonNetwork.NickName = defaultName;
	}

	public void SetPlayerName(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
            Debug.LogError("Player Name is null or empty");
		    return;
		}
		PhotonNetwork.NickName = value;

		PlayerPrefs.SetString(playerNamePrefKey, value);
	}
}