using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KinematicCharacterController.Examples;
using UnityEngine.SceneManagement;
using KinematicCharacterController;
using Cinemachine;
using UnityEngine.InputSystem.Controls;
using Photon.Pun;
using Smooth;

public class KPlayer : MonoBehaviour
{
	[SerializeField] GameObject playerUIPrefab;
	public PhotonView pv;
	public SmoothSyncPUN2 smoothSync;
	public CharacterModel characterModel;
	public GameObject PlayerCharacter;
	public int skinNumber;
	public KCharacterController kcc;
	public KAnimator kanim;
	public KPickup kpickup;
	public ABC_StateManager ABCEvents;
	public ABC_Controller ABCcontroller;
	public CinemachineVirtualCamera cvm;
	public string Name = "Guest";
	public Vector2 MovementStickValue;
	public bool Jump;
	public bool Crouch;
	public LayerMask PlayerMask;
	private GameObject objectInSight;
	private GameObject pickedUpObject;
	private bool PressedHoldThrow;
	private RaycastHit hit;
	private Vector3 rayOrigin;
	private Vector3 forward;
	private KInputActions controls;
	private float assistRadius = 0.25f;
	private float pickupDist = 0.5f;
	private float currentHitDistance;
	private float aimRadius = 0.5f;
	public float heat = 0f;
	private float heatBar = 0f;

	public void OnEnable()
	{
		if (!pv.IsMine)
			return;
		if (controls != null)
			controls.Enable();
		if (ABCEvents != null)
		{
			ABCEvents.onEnableMovement += EnableMovement;
			ABCEvents.onDisableMovement += DisableMovement;

			ABCEvents.onEnableGravity += EnableGravity;
			ABCEvents.onDisableGravity += DisableGravity;
		}
	}

	public void OnDisable()
	{
		if (!pv.IsMine)
			return;
		if (controls != null)
			controls.Disable();
		if (ABCEvents != null)
		{
			ABCEvents.onEnableMovement -= EnableMovement;
			ABCEvents.onDisableMovement -= DisableMovement;

			ABCEvents.onEnableGravity -= EnableGravity;
			ABCEvents.onDisableGravity -= DisableGravity;
		}
	}

	public void EnableMovement()
	{
		kcc.RestrictMovement = false;
	}

	public void DisableMovement()
	{
		kcc.RestrictMovement = true;
	}

	public void EnableGravity()
	{
		kcc.DefyGravity = false;
	}

	public void DisableGravity()
	{
		kcc.DefyGravity = true;
	}

	public void Awake()
	{
		PlayerCharacter = Instantiate(characterModel.characterModel[skinNumber], transform);
		kanim.anim.avatar = PlayerCharacter.GetComponent<Animator>().avatar;

		if (!pv.IsMine)
			return;
		controls = new KInputActions();
		controls.Player.Move.performed += context =>
		{
			MovementStickValue = context.ReadValue<Vector2>();
		};
		controls.Player.Jump.performed += context =>
		{
			if (Crouch)
				return;
			var button = context.control as ButtonControl;
			if (button.wasPressedThisFrame)
				JumpPressed();
			if (button.wasReleasedThisFrame)
				JumpReleased();
		};
		controls.Player.Crouch.performed += context =>
		{
			var button = context.control as ButtonControl;
			if (button.wasPressedThisFrame)
				ActionPressed();
			if (button.wasReleasedThisFrame)
				ActionReleased();
		};
		controls.Player.DodgeLeft.performed += context =>
		{
			var button = context.control as ButtonControl;
			if (button.wasPressedThisFrame)
				DashPressed();
		};
		controls.Player.DodgeRight.performed += context =>
		{
			var button = context.control as ButtonControl;
			if (button.wasPressedThisFrame)
				DashPressed();
		};
		controls.UI.Exit.performed += context =>
		{
			ExitPressed();
		};
	}

	private void JumpPressed()
	{
		if (kpickup)
			StartThrow(-1);
		else
			Jump = true;
	}

	private void JumpReleased()
	{
		Jump = false;
	}

	public void ActionPressed()
	{
		if (kpickup)
		{
			PressedHoldThrow = true;
		}
		else
		{
			if (objectInSight == null)
			{
				Crouch = true;
			}
			else
			{
				StartPickup();
			}
		}
	}

	public void ActionReleased()
	{
		if (kpickup)
		{
			if (heat > 0)
				StartThrow(1);
		}
		Crouch = false;
	}

	private void StartThrow(int direction)
	{
		if (direction == 1)
		{
			kanim.anim.SetTrigger("Throw");
		}
		else if (direction == -1)
		{
			kanim.anim.SetTrigger("Suplex");
		}
		PressedHoldThrow = false;
	}

	public void ThrowPickup(int direction)
	{
		if (pickedUpObject)
		{
			pv.RPC("syncThrowPickup", RpcTarget.All);
		}
	}

	[PunRPC]
	public void syncThrowPickup()
	{
		pickedUpObject = null;
		kpickup = null;
		kanim.anim.SetBool("PickUp", false);
		kanim.anim.SetBool("Carrying", false);
		heat = 0f;
		heatBar = heat / kcc.maxWarmup;
	}

	private void DashPressed()
	{
		//if (kcc.Motor.GroundingStatus.IsStableOnGround)
		//	ABCcontroller.TriggerAbility(1034061);
		//else
		//	ABCcontroller.TriggerAbility(1034021);
	}

	private void ExitPressed()
	{
		KGameManager.Instance.playerSpawner.DisconnectPlayer();
	}

	[PunRPC]
	public void syncStartPickup()
	{
		kanim.anim.SetBool("PickUp", true);
		StartCoroutine("PickupObject");
	}

	IEnumerator PickupObject()
	{
		yield return new WaitForSeconds(0.5f);
		if (kpickup)
		{
			kanim.anim.SetBool("PickUp", false);
			kanim.anim.SetBool("Carrying", true);
		}
	}

	public void StartPickup()
	{
		pickedUpObject = objectInSight;
		kpickup = pickedUpObject.GetComponent<KPickup>();
		pv.RPC("syncStartPickup", RpcTarget.All);
	}

	private void Start()
	{
		Name = pv.Owner.NickName;
		playerUIPrefab = Instantiate(playerUIPrefab);
		playerUIPrefab.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
	}

	private void Update()
	{
		if (!pv.IsMine)
			return;
		forward = transform.TransformDirection(Vector3.forward);
		rayOrigin = transform.position + transform.up * 0.25f;
		if (CheckRaycast())
		{
			objectInSight = hit.transform.gameObject;
		}
		else
		{
			objectInSight = null;
		}
		if (PressedHoldThrow)
		{
			if (heat < kcc.maxWarmup)
			{
				heat += Time.deltaTime;
				heatBar = heat / kcc.maxWarmup;
			}
		}
	}

	public bool CheckRaycast()
	{
		if (Physics.SphereCast(rayOrigin, assistRadius, forward, out hit, pickupDist, PlayerMask))
			return true;
		return false;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Debug.DrawLine(rayOrigin, rayOrigin + forward * pickupDist);
		Gizmos.DrawWireSphere(rayOrigin + forward * pickupDist, assistRadius);

		Gizmos.color = Color.yellow;
		Debug.DrawLine(rayOrigin, rayOrigin + forward * currentHitDistance);
		Gizmos.DrawWireSphere(rayOrigin + forward * currentHitDistance, aimRadius);
	}
}
