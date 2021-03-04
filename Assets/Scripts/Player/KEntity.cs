using KinematicCharacterController;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KEntity : MonoBehaviour
{
	[SerializeField] KinematicCharacterMotor Motor;
	[SerializeField] PhotonView pv;
	[SerializeField] ABC_StateManager abcState;
	[SerializeField] KPlayer kPlayer;
	[SerializeField] KPickup kPickup;
	private Vector3 spawnPoint;
	private Quaternion spawnRotation;

	private void Awake()
	{
		spawnPoint = Motor.TransientPosition;
		spawnRotation = Motor.TransientRotation;
	}

	private void Update()
	{
		if (Motor.TransientPosition.y < -5)
		{
			kPickup.pv.RPC("SyncPickup", RpcTarget.All, kPlayer.playerIndex);
			kPickup.pv.RPC("SyncThrow", RpcTarget.All, kPlayer.playerIndex, 0f, transform.position, transform.rotation.eulerAngles.y);
			Motor.SetPositionAndRotation(spawnPoint, spawnRotation);
		}
	}

	public void Damage(int val)
	{
		pv.RPC("SyncDamage", RpcTarget.All, val);
	}

	[PunRPC]
	void SyncDamage(int val)
	{
		abcState.AdjustHealth(val);
		AudioManager.Instance.PlaySFX(3, Motor.TransientPosition);
	}
}
