using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KGameManager : MonoBehaviour
{
	[SerializeField] public PhotonView pv;
	[SerializeField] public PlayerSpawner playerSpawner;
	[SerializeField] public GameObject playerUIPanel;
	[SerializeField] public CinemachineVirtualCamera cvm;
	[SerializeField] public CinemachineTargetGroup cvgroup;
	public List<KPlayer> kPlayers;
	private static KGameManager instance;

	public static KGameManager Instance
	{
		get
		{
			return instance;
		}
	}

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
