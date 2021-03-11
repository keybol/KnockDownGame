using KinematicCharacterController;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class KEntity : MonoBehaviour
{
	[SerializeField] KinematicCharacterMotor Motor;
	[SerializeField] PhotonView pv;
	[SerializeField] ABC_StateManager abcState;
	[SerializeField] KPlayer kPlayer;
	[SerializeField] KPickup kPickup;
	public float invincibleTime = 0f;
	public bool isInvincible;
	public float timeToInvincible = 5f;
	public bool startIFrames;
	public float flashCounter = 0;
	public float flashLength = 0.15f;
	private Vector3 spawnPosition;
	private Quaternion spawnRotation;

	private void Awake()
	{
		spawnPosition = Motor.TransientPosition;
		spawnRotation = Motor.TransientRotation;
	}

	private void Update()
	{
		if (!kPlayer)
			return;
		if (Motor.TransientPosition.y < -5)
		{
			kPickup.pv.RPC("SyncPickup", RpcTarget.All, kPlayer.playerIndex);
			kPickup.pv.RPC("SyncThrow", RpcTarget.All, kPlayer.playerIndex, 0f, spawnPosition, spawnRotation.eulerAngles.y);
			Motor.SetPositionAndRotation(spawnPosition, spawnRotation);
		}
		if (isInvincible)
		{
			SetCharacterBlinking();
		}
	}

	void SetCharacterBlinking()
	{
		if (startIFrames)
		{
			flashCounter -= Time.deltaTime;
			if (flashCounter <= 0)
			{
				for (int i = 0; i < kPlayer.charRenderer.Length; i++)
				{
					kPlayer.charRenderer[i].enabled = !kPlayer.charRenderer[i].enabled;
				}
				flashCounter = flashLength;
			}
		}
		if (Time.time >= invincibleTime)
		{
			for (int i = 0; i < kPlayer.charRenderer.Length; i++)
			{
				kPlayer.charRenderer[i].enabled = true;
			}
			isInvincible = false;
			startIFrames = false;
			kPlayer.kcc.IgnoredColliders.Clear();
		}
	}

	public void Damage(int val, Vector3 hitPoint)
	{
		if (isInvincible)
			return;
		pv.RPC("SyncDamage", RpcTarget.All, val, hitPoint);
	}

	[PunRPC]
	void SyncDamage(int val, Vector3 hitPoint)
	{
		hitPoint.y = 4;
		kPlayer.kcc.Motor.ForceUnground();
		kPlayer.kcc.AddVelocity(hitPoint);
		//kPlayer.bzRagdoll.RagdollIn();
		//kPlayer.bzRagdoll.RagdollOut();
		//List<Collider> playercolliders = GameObject.FindGameObjectsWithTag("Pickup").Select(obj => obj.GetComponent<Collider>()).ToList();
		//kPlayer.kcc.IgnoredColliders.AddRange(playercolliders);
		//kPlayer.bzRagdoll.AddExtraMove(hitPoint);
		abcState.AdjustHealth(val);
		AudioManager.Instance.PlaySFX(3, Motor.TransientPosition);
		isInvincible = true;
		startIFrames = true;
		invincibleTime = Time.time + timeToInvincible;
	}
}
