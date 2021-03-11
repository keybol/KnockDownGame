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
using Photon.Realtime;
using UnityEngine.AI;
using UnityEngine.UI;
using BzKovSoft.RagdollTemplate.Scripts.Charachter;

public class KPlayer : MonoBehaviour, IPunInstantiateMagicCallback
{
	[SerializeField] GameObject playerUIPrefab;
	public Collider myCollider;
	public PhotonView pv;
	public SmoothSyncPUN2 smoothSync;
	public CharacterModel[] characterModel;
	public GameObject PlayerCharacter;
	public NavMeshAgent navMesh;
	public AINavigation aiNav;
	public ABC_StateManager abcState;
	public ABC_Controller abcController;
	public KEntity kEntity;
	public BzRagdoll bzRagdoll;
	public Transform _hipsTransform;
	public Rigidbody _hipsTransformRigid;
	public Renderer[] charRenderer;
	public FixedJoint fixedJoint;
	public Rigidbody rb;
	public bool humanPlayer;
	public int characterNumber;
	public int skinNumber;
	public string botName;
	public int botCharacterNumber;
	public int botSkinNumber;
	public int playerIndex;
	public KCharacterController kcc;
	public KAnimator kanim;
	public KPickup playerKPickup;
	public KPickup kpickup;
	public CinemachineVirtualCamera cvm;
	public Transform itemAnchor;
	public string Name = "Guest";
	public Vector2 MovementStickValue;
	public bool Jump;
	public bool Crouch;
	public bool Carried;
	public LayerMask PlayerMask;
	public GameObject objectInSight;
	public uint outlineRenderingLayerMask;
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
	private Sprite jumpButtonSprite;
	private Sprite suplexButtonSprite;
	private Sprite crouchButtonSprite;
	private Sprite pickupButtonSprite;
	private Sprite throwButtonSprite;
	private Image jumpButtonImage;
	private Image crouchButtonImage;
	private Image specialButtonImage;

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		object[] objecttype = pv.InstantiationData;
		playerIndex = (int)objecttype[0];
		if (objecttype.Length == 4)
		{
			humanPlayer = false;
			botName = (string)objecttype[1];
			botCharacterNumber = (int)objecttype[2];
			botSkinNumber = (int)objecttype[3];
		}
		KGameManager.Instance.kPlayers[playerIndex] = this;
	}

	public void OnEnable()
	{
		if (!pv.IsMine)
			return;
		if (controls != null)
			controls.Enable();
		if (abcState != null)
		{
			abcState.onEnableMovement += EnableMovement;
			abcState.onDisableMovement += DisableMovement;

			abcState.onEnableGravity += EnableGravity;
			abcState.onDisableGravity += DisableGravity;
		}
	}

	public void OnDisable()
	{
		if (!pv.IsMine)
			return;
		if (controls != null)
			controls.Disable();
		if (abcState != null)
		{
			abcState.onEnableMovement -= EnableMovement;
			abcState.onDisableMovement -= DisableMovement;

			abcState.onEnableGravity -= EnableGravity;
			abcState.onDisableGravity -= DisableGravity;

			abcState.onEffectActivation -= EffectActivation;
		}
	}

	public void EffectActivation(Effect Effect, ABC_IEntity Target, ABC_IEntity Originator)
	{
		Debug.Log("DODGE ACTIVATE");
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

	public void EffectActivate(Effect Effect, ABC_IEntity Target, ABC_IEntity Originator)
	{

	}

	public void Awake()
	{
		//playerKPickup.enabled = false;
		if (!pv.IsMine)
			return;
		jumpButtonSprite = KGameManager.Instance.jumpButtonSprite;
		suplexButtonSprite = KGameManager.Instance.suplexButtonSprite;
		crouchButtonSprite = KGameManager.Instance.crouchButtonSprite;
		pickupButtonSprite = KGameManager.Instance.pickupButtonSprite;
		throwButtonSprite = KGameManager.Instance.throwButtonSprite;
		jumpButtonImage = KGameManager.Instance.jumpButtonImage;
		crouchButtonImage = KGameManager.Instance.crouchButtonImage;
		specialButtonImage = KGameManager.Instance.specialButtonImage;
		controls = new KInputActions();
		controls.Player.Jump.performed += context =>
		{
			if (Crouch || !humanPlayer)
				return;
			var button = context.control as ButtonControl;
			if (button.wasPressedThisFrame)
				JumpPressed();
			if (button.wasReleasedThisFrame)
				JumpReleased();
		};
		controls.Player.Crouch.performed += context =>
		{
			if (!humanPlayer)
				return;
			var button = context.control as ButtonControl;
			if (button.wasPressedThisFrame)
				ActionPressed();
			if (button.wasReleasedThisFrame)
				ActionReleased();
		};
		controls.Player.DodgeLeft.performed += context =>
		{
			if (!humanPlayer)
				return;
			var button = context.control as ButtonControl;
			if (button.wasPressedThisFrame)
				DodgePressed();
		};
		controls.Player.DodgeRight.performed += context =>
		{
			if (!humanPlayer)
				return;
			var button = context.control as ButtonControl;
			if (button.wasPressedThisFrame)
				DodgePressed();
		};
		controls.UI.Exit.performed += context =>
		{
			if (!humanPlayer)
				return;
			ExitPressed();
		};
	}

	private void JumpPressed()
	{
		if (kpickup)
		{
			PressedHoldThrow = true;
		}
		else
			Jump = true;
	}

	private void JumpReleased()
	{
		if (kpickup)
		{
			if (heat > 0)
				StartThrow(-1);
		}
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
		if (kpickup)
		{
			float heatThrowPower = kcc.MinThrowPower + heat * kcc.ThrowPower;
			Vector3 throwPosition = kpickup.transform.position;
			if(direction == -1)
				throwPosition.y = transform.position.y + 1.02f;
			if(kpickup.pickupKPlayer)
				throwPosition.y = transform.position.y + 1.52f;
			crouchButtonImage.sprite = crouchButtonSprite;
			jumpButtonImage.sprite = jumpButtonSprite;
			kpickup.pv.RPC("SyncThrow", RpcTarget.All, playerIndex, heatThrowPower * direction, throwPosition, kpickup.transform.rotation.eulerAngles.y);
			pv.RPC("syncThrowPickup", RpcTarget.All);
		}
	}

	[PunRPC]
	public void syncThrowPickup()
	{
		AudioManager.Instance.PlaySFX(2, kcc.Motor.TransientPosition);
		kanim.anim.SetBool("PickUp", false);
		kanim.anim.SetBool("Carrying", false);
		heat = 0f;
		kpickup = null;
	}

	private void DodgePressed()
	{
		bzRagdoll.RagdollIn();
		bzRagdoll.RagdollOut();
	}

	private void ExitPressed()
	{
		KGameManager.Instance.playerSpawner.DisconnectPlayer();
	}

	[PunRPC]
	public void syncStartPickup()
	{
		StartCoroutine("PickupObject");
	}

	IEnumerator PickupObject()
	{
		kanim.anim.SetBool("PickUp", true);
		yield return new WaitForSeconds(0.5f);
		kanim.anim.SetBool("PickUp", false);
		kanim.anim.SetBool("Carrying", true);
		AudioManager.Instance.PlaySFX(4, kcc.Motor.TransientPosition);
	}

	public void StartPickup()
	{
		crouchButtonImage.sprite = throwButtonSprite;
		jumpButtonImage.sprite = suplexButtonSprite;
		kpickup = objectInSight.GetComponent<KPickup>();
		kpickup.pv.RPC("SyncPickup", RpcTarget.All, playerIndex);
		pv.RPC("syncStartPickup", RpcTarget.All);
	}

	private void Start()
	{
		KGameManager.Instance.cvgroup.m_Targets[playerIndex].target = transform;
		KGameManager.Instance.cvgroup.m_Targets[playerIndex].radius = 0.25f;
		if (humanPlayer)
		{
			KGameManager.Instance.cvgroup.m_Targets[playerIndex].radius = 4;
			characterNumber = (int)pv.Owner.CustomProperties["CharacterNumber"];
			skinNumber = (int)pv.Owner.CustomProperties["SkinNumber"];
			Name = pv.Owner.NickName;
			aiNav.enabled = false;
			navMesh.enabled = false;
		}
		else
		{
			characterNumber = botCharacterNumber;
			skinNumber = botSkinNumber;
			Name = botName;
		}
		PlayerCharacter = Instantiate(characterModel[characterNumber].characterModel[skinNumber], transform);
		kanim.anim.avatar = PlayerCharacter.GetComponent<Animator>().avatar;
		itemAnchor = kanim.anim.GetBoneTransform(HumanBodyBones.Head);
		_hipsTransform = kanim.anim.GetBoneTransform(HumanBodyBones.Hips);
		_hipsTransformRigid = _hipsTransform.GetComponent<Rigidbody>();
		playerUIPrefab = Instantiate(playerUIPrefab);
		playerUIPrefab.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
		outlineRenderingLayerMask = PlayerCharacter.GetComponentInChildren<Renderer>().renderingLayerMask;
		charRenderer = PlayerCharacter.GetComponentsInChildren<Renderer>();
		fixedJoint.connectedBody = _hipsTransformRigid;
		if (!pv.IsMine)
		{
			smoothSync.hasRigidbody = false;
			Destroy(fixedJoint);
			Destroy(rb);
		}
	}

	public void EnableSmoothSync()
	{
		if (!humanPlayer)
		{
			abcController.enabled = true;
			aiNav.enabled = true;
			navMesh.enabled = true;
		}
		smoothSync.enabled = true;
		kcc.RestrictMovement = false;
	}

	private void Update()
	{
		if (!pv.IsMine)
			return;
		forward = transform.TransformDirection(Vector3.forward);
		rayOrigin = transform.position + transform.up * 0.25f;
		if (CheckValidPickup())
		{
			objectInSight = hit.transform.gameObject;
		}
		else
		{
			objectInSight = null;
		}

		if (humanPlayer)
		{
			MovementStickValue = controls.Player.Move.ReadValue<Vector2>();
			if (kpickup == null)
			{
				if (objectInSight == null)
				{
					crouchButtonImage.sprite = crouchButtonSprite;
				}
				else
				{
					crouchButtonImage.sprite = pickupButtonSprite;
					if (objectInSight.GetComponent<KPlayer>())
					{
						foreach (Renderer go in objectInSight.GetComponent<KPlayer>().charRenderer)
						{
							go.renderingLayerMask = outlineRenderingLayerMask;
						}
					}
					else
					{
						foreach (Renderer go in objectInSight.GetComponent<KPickup>().PickupObject.GetComponentsInChildren<Renderer>())
						{
							go.renderingLayerMask = outlineRenderingLayerMask;
						}
					}
				}				
			}
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

	public bool CheckValidPickup()
	{
		if (Physics.SphereCast(rayOrigin, assistRadius, forward, out hit, pickupDist, PlayerMask))
		{
			if (hit.transform.GetComponent<KPlayer>())
			{
				if (hit.transform.GetComponent<KEntity>().isInvincible)
					return false;
				if (hit.transform.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsTag("GettingUp"))
					return false;
			}
			return true;
		}
		return false;
	}

	public void SuplexHitEffect()
	{
		MultiFunction.DoVibrate(MoreMountains.NiceVibrations.HapticTypes.HeavyImpact);
		AudioManager.Instance.PlaySFX(UnityEngine.Random.Range(7, 9), kcc.Motor.TransientPosition);
		GameObject landSmoke = ObjectPoolerManager.Instance.GetPooledLandSmokeObject();
		landSmoke.transform.position = kpickup.transform.position;
		landSmoke.SetActive(true);
		ScreenShaker.Instance.ShakeScreen(0.2f);
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
