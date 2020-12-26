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

	private void Awake()
	{
		Motor.CharacterController = this;
	}

	[PunRPC]
	public void SyncPickup(int _playerIndex, Vector3 _position)
	{
		Motor.enabled = false;
		KPlayer kplayer = KGameManager.Instance.kPlayers[_playerIndex];
		kplayer.kcc.IgnoredColliders.Add(this.GetComponent<Collider>());
		transform.parent = kplayer.itemAnchor;
		transform.localPosition = new Vector3(0, kplayer.kcc.ThrowHeight, 0f);
	}

	[PunRPC]
	public void SyncThrow(int _playerIndex, float _heatThrowPower, Vector3 _position, Vector3 _rotation)
	{
		KPlayer kplayer = KGameManager.Instance.kPlayers[_playerIndex];
		Vector3 finalPos = transform.position;
		if (_heatThrowPower < 0)
			finalPos.y = kplayer.itemAnchor.position.y + kplayer.kcc.ThrowHeight;
		Motor.SetPosition(finalPos, true);
		Motor.SetRotation(Quaternion.Euler(_rotation), true);
		Motor.enabled = true;
		kplayer.kcc.IgnoredColliders.Remove(this.GetComponent<Collider>());
		Motor.BaseVelocity = transform.forward * _heatThrowPower;
		transform.parent = null;
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
		// Handle landing and leaving ground
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
		Motor.BaseVelocity = Vector3.zero;
	}

	protected void OnLeaveStableGround()
	{

	}
}
