using KinematicCharacterController;
using Photon.Pun;
using Smooth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KPickup : MonoBehaviour, ICharacterController
{
	[SerializeField] KinematicCharacterMotor Motor;
	public List<Collider> IgnoredColliders = new List<Collider>();
	public PhotonView pv;
	public Vector3 Gravity = new Vector3(0, -30f, 0);
	public KPlayer pickupKPlayer;
	private KPlayer kplayer;

	private void Awake()
	{
		Motor.CharacterController = this;
	}

	[PunRPC]
	public void SyncPickup(int _playerIndex)
	{
		if (pickupKPlayer)
		{
			pickupKPlayer.Carried = true;
			pickupKPlayer.smoothSync.enabled = false;
			pickupKPlayer.kcc.enabled = false;
			pickupKPlayer.enabled = false;
			Motor.CharacterController = this;
		}
		Motor.enabled = false;
		kplayer = KGameManager.Instance.kPlayers[_playerIndex];
		kplayer.kcc.IgnoredColliders.Add(this.GetComponent<Collider>());
		transform.parent = kplayer.itemAnchor;
		transform.localPosition = new Vector3(0, kplayer.kcc.ThrowHeight, 0f);
		transform.localRotation = Quaternion.Euler(Vector3.zero);
		if (pickupKPlayer)
		{
			transform.localPosition = new Vector3(0, 0.5f, 0);
			transform.localRotation = Quaternion.Euler(new Vector3(0, -90, 0f));
		}
	}

	[PunRPC]
	public void SyncThrow(int _playerIndex, float _heatThrowPower, Vector3 _position, float _rotationY)
	{
		kplayer = KGameManager.Instance.kPlayers[_playerIndex];
		Motor.SetPosition(_position, true);
		Motor.SetRotation(Quaternion.Euler(new Vector3(0, _rotationY, 0)), true);
		Motor.enabled = true;
		Motor.BaseVelocity = kplayer.transform.forward * _heatThrowPower;
		StartCoroutine("IgnorePlayerCollider");
		transform.parent = null;
	}

	IEnumerator IgnorePlayerCollider()
	{
		yield return new WaitForSeconds(0.5f);
		kplayer.kcc.IgnoredColliders.Remove(this.GetComponent<Collider>());
	}

	public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
	{

	}

	public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
	{
		currentVelocity += Gravity * deltaTime;
	}

	public void BeforeCharacterUpdate(float deltaTime)
	{

	}

	public void PostGroundingUpdate(float deltaTime)
	{
		if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
		{
			OnLanded();
		}
		else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
		{
			OnLeaveStableGround();
		}
	}

	public void AfterCharacterUpdate(float deltaTime)
	{
		
	}

	public bool IsColliderValidForCollisions(Collider coll)
	{
		if (IgnoredColliders.Count == 0)
		{
			return true;
		}

		if (IgnoredColliders.Contains(coll))
		{
			return false;
		}

		return true;
	}

	public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
		
	}

	public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
	}

	public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
	{
		
	}

	public void OnDiscreteCollisionDetected(Collider hitCollider)
	{
		
	}

	protected void OnLanded()
	{
		pv.RPC("SyncOnPickupLanded", RpcTarget.All, transform.position);
	}

	[PunRPC]
	public void SyncOnPickupLanded(Vector3 _position)
	{
		Motor.BaseVelocity = Vector3.zero;
		if (pickupKPlayer)
		{
			pickupKPlayer.Carried = false;
			pickupKPlayer.smoothSync.enabled = true;
			pickupKPlayer.kcc.enabled = true;
			pickupKPlayer.enabled = true;
			Motor.SetPosition(_position, true);
			Motor.CharacterController = pickupKPlayer.kcc;
		}
		GameObject landSmoke = ObjectPoolerManager.Instance.GetPooledObject();
		if (landSmoke != null)
		{
			landSmoke.transform.position = _position;
			landSmoke.SetActive(true);
		}
	}

	protected void OnLeaveStableGround()
	{

	}
}
