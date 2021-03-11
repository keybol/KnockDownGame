using KinematicCharacterController;
using Photon.Pun;
using Smooth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KPickup : MonoBehaviour, ICharacterController
{
	[SerializeField] KinematicCharacterMotor Motor;
	public GameObject PickupObject;
	public List<Collider> IgnoredColliders = new List<Collider>();
	public PhotonView pv;
	public Vector3 Gravity = new Vector3(0, -30f, 0);
	public bool isThrown;
	public KPlayer pickupKPlayer;
	private KPlayer kplayer;
	private KEntity kEntity;
	private float impactSpeed = 2f;

	private void Awake()
	{
		Motor.CharacterController = this;
	}

	private void FixedUpdate()
	{
		if (pickupKPlayer)
		{
			foreach (Renderer go in pickupKPlayer.PlayerCharacter.GetComponentsInChildren<Renderer>())
			{
				go.renderingLayerMask = 1;
			}
		}
		else
		{
			foreach (Renderer go in PickupObject.GetComponentsInChildren<Renderer>())
			{
				go.renderingLayerMask = 1;
			}
		}
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
			if (!pickupKPlayer.humanPlayer)
			{
				pickupKPlayer.abcController.enabled = false;
				pickupKPlayer.aiNav.enabled = false;
				pickupKPlayer.navMesh.enabled = false;
			}
			Motor.CharacterController = this;
		}
		Motor.enabled = false;
		kplayer = KGameManager.Instance.kPlayers[_playerIndex];
		kplayer.kcc.IgnoredColliders.Add(this.GetComponent<Collider>());
		transform.parent = kplayer.itemAnchor;
		transform.localPosition = new Vector3(0, -0.25f, 0f);
		transform.localRotation = Quaternion.Euler(Vector3.zero);
		if (pickupKPlayer)
		{
			transform.localPosition = new Vector3(0, 0.25f, 0);
			transform.localRotation = Quaternion.Euler(new Vector3(0, -90, 0f));
		}
	}

	[PunRPC]
	public void SyncThrow(int _playerIndex, float _heatThrowPower, Vector3 _position, float _rotationY)
	{
		isThrown = true;
		kplayer = KGameManager.Instance.kPlayers[_playerIndex];
		Motor.SetPosition(_position, true);
		Motor.SetRotation(Quaternion.Euler(new Vector3(0, _rotationY, 0)), true);
		Motor.enabled = true;
		Motor.BaseVelocity = kplayer.transform.forward * _heatThrowPower;
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
		if (hitCollider.gameObject.GetComponent<KEntity>())
		{
			Debug.Log("col.transform.tag" + hitCollider.transform.tag);
			Motor.BaseVelocity = hitCollider.transform.position - transform.position;
		}
	}

	public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
		if (hitCollider.gameObject.GetComponent<KPlayer>() && isThrown)
		{
			ScreenShaker.Instance.ShakeScreen(0.2f);
			Vector3 impact = hitCollider.transform.position - transform.position;
			hitCollider.gameObject.GetComponent<KEntity>().Damage(-10, impact.normalized * impactSpeed);
			isThrown = false;
		}
	}

	public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
	{
		
	}

	public void OnDiscreteCollisionDetected(Collider hitCollider)
	{
		
	}

	protected void OnLanded()
	{
		if(pv.IsMine)
			pv.RPC("SyncOnPickupLanded", RpcTarget.All, transform.position);
	}

	[PunRPC]
	public void SyncOnPickupLanded(Vector3 _position)
	{
		if (!kplayer)
			return;
		isThrown = false;
		Motor.BaseVelocity = Vector3.zero;
		kplayer.kcc.IgnoredColliders.Remove(this.GetComponent<Collider>());
		if (pickupKPlayer)
		{
			ScreenShaker.Instance.ShakeScreen(0.2f);
			pickupKPlayer.Carried = false;
			//pickupKPlayer.kanim.anim.Play("GetUp.StandUpFromBack");
			pickupKPlayer.smoothSync.enabled = true;
			pickupKPlayer.kcc.enabled = true;
			pickupKPlayer.enabled = true;
			pickupKPlayer.kEntity.Damage(-10, Vector3.zero);
			Motor.CharacterController = pickupKPlayer.kcc;
		}
		GameObject landSmoke = ObjectPoolerManager.Instance.GetPooledLandSmokeObject();
		landSmoke.transform.position = _position;
		landSmoke.SetActive(true);
	}

	protected void OnLeaveStableGround()
	{

	}
}
