﻿using KinematicCharacterController;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KEntity : MonoBehaviour
{
	[SerializeField] KinematicCharacterMotor Motor;
	[SerializeField] PhotonView pv;
	[SerializeField] ABC_StateManager abcState;
	private Vector3 spawnPoint;
	private Quaternion spawnRotation;

	private void Awake()
	{
		spawnPoint = Motor.TransientPosition;
		spawnRotation = Motor.TransientRotation;
	}

	private void Update()
	{
		if (transform.position.y < -5)
			Motor.SetPositionAndRotation(spawnPoint, spawnRotation);
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