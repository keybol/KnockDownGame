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

public class KPlayer : MonoBehaviour
{
	public GameObject PlayerSkin;
	public KCharacterController kcc;
	public ABC_StateManager ABCEvents;
	public ABC_Controller ABCcontroller;
	public CinemachineVirtualCamera cvm;
	private KInputActions controls;
	public Transform CameraFollowPoint;
	public Transform CameraFollowParent;
	public GameObject ControlsPanel;
	public Vector2 MovementStickValue;
	public Vector2 lookInputVector;
	public bool Jump;
	public bool Attack;
	public bool Dash;
	public float rotationPower = 10f;
	public Vector3 angles;
	public Vector3 PlanarDirection;
	public float minAngle = 40;
	public float maxAngle = 340;
	public float RotationSharpness = 10000f;
	public bool InvertX = false;
	public bool InvertY = false;
	public float MaxStableAngle = 45;
	private float _targetVerticalAngle;
	private Vector3 _currentFollowPosition;
	Vector3 velocity = Vector3.zero;
	public float dampTime = 0.01f;
	Vector3 pastFollowerPosition;
	Vector3 pastTargetPosition;
	private bool holdInput;
	private bool isMobile;

	public void OnEnable()
	{
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
		if (controls != null)
			controls.Disable();
		if (ABCEvents != null)
		{
			ABCEvents.onEnableMovement -= EnableMovement;
			ABCEvents.onDisableMovement -= DisableMovement;

			ABCEvents.onEnableGravity -= EnableGravity;
			ABCEvents.onDisableGravity -= DisableGravity;

			ABCEvents.onEffectActivation -= EffectActivation;
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
		isMobile = Application.isMobilePlatform;
		//isMobile = true;
		PlanarDirection = Vector3.forward;
		controls = new KInputActions();
		controls.Player.Move.performed += context =>
		{
			MovementStickValue = context.ReadValue<Vector2>();
		};
		controls.Player.LookGamepad.performed += context =>
		{
			lookInputVector = context.ReadValue<Vector2>();
		};
		controls.Player.LookGamepad.canceled += context =>
		{
			lookInputVector = Vector2.zero;
		};
		if (isMobile)
		{
			controls.Player.LookMobile.performed += context =>
			{
				if (holdInput)
					lookInputVector = context.ReadValue<Vector2>();
			};
			controls.Player.LookMobile.canceled += context =>
			{
				lookInputVector = Vector2.zero;
			};
		}
		else
		{
			controls.Player.Look.performed += context =>
			{
				lookInputVector = context.ReadValue<Vector2>();
			};
			controls.Player.Look.canceled += context =>
			{
				lookInputVector = Vector2.zero;
			};
			controls.Player.LMB.performed += context =>
			{
				var button = context.control as ButtonControl;
				if (button.wasPressedThisFrame)
					AttackPressed();
			};
			controls.Player.RMB.performed += context =>
			{
				var button = context.control as ButtonControl;
				if (button.wasPressedThisFrame)
					DashPressed();
			};
		}
		controls.Player.Jump.performed += context =>
		{
			var button = context.control as ButtonControl;
			if (button.wasPressedThisFrame)
				Jump = true;
			if (button.wasReleasedThisFrame)
				Jump = false;
		};
		controls.Player.Fire1.performed += context =>
		{
			var button = context.control as ButtonControl;
			if (button.wasPressedThisFrame)
				AttackPressed();
		};
		controls.Player.Fire2.performed += context =>
		{
			var button = context.control as ButtonControl;
			if (button.wasPressedThisFrame)
				DashPressed();
		};
		controls.Player.RightInputHold.performed += context =>
		{
			var button = context.control as ButtonControl;
			if (button.wasPressedThisFrame)
				holdInput = true;
			if (button.wasReleasedThisFrame)
				holdInput = false;
		};
		controls.UI.Exit.performed += context =>
		{
			ExitPressed();
		};
	}

	private void AttackPressed()
	{
		//ABCcontroller.TriggerAbility(1034891);
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
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	void Start()
	{
		//CameraFollowParent.transform.position = transform.position;
		if (isMobile)
		{

		}
		else
		{
			Cursor.lockState = CursorLockMode.Locked;
			//ControlsPanel.SetActive(false);
		}
	}

	void Update()
	{
		if (transform.position.y < -100)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

	private void LateUpdate()
	{
		UpdateWithInput(Time.deltaTime, lookInputVector);
	}

	Vector3 SmoothApproach(Vector3 pastPosition, Vector3 pastTargetPosition, Vector3 targetPosition, float speed)
	{
		float t = Time.deltaTime * speed;
		Vector3 v = (targetPosition - pastTargetPosition) / t;
		Vector3 f = pastPosition - pastTargetPosition + v;
		return targetPosition - v + f * Mathf.Exp(-t);
	}

	public void UpdateWithInput(float deltaTime, Vector3 rotationInput)
	{
		if (InvertX)
		{
			//lookInputVector.x *= -1f;
		}
		if (InvertY)
		{
			//lookInputVector.y *= -1f;
		}

		//cvm.GetCinemachineComponent<Cinemachine3rdPersonFollow>().Damping = kcc.Motor.CharacterUp + new Vector3(0f, -1f, 1f);

		//CameraFollowParent.transform.position = transform.position;
		//CameraFollowParent.position = SmoothApproach(pastFollowerPosition, pastTargetPosition, CameraFollowParent.position, dampTime);
		//pastFollowerPosition = CameraFollowParent.position;
		//pastTargetPosition = transform.position;

		//CameraFollowParent.transform.rotation = transform.rotation;
		//CameraFollowParent.transform.rotation = Quaternion.Slerp(CameraFollowParent.transform.rotation, transform.rotation, kcc.PullDamping);

		// Process rotation input
		//Quaternion rotationFromInput = Quaternion.Euler(CameraFollowParent.up * (lookInputVector.x * rotationPower));
		//PlanarDirection = rotationFromInput * PlanarDirection;
		//PlanarDirection = Vector3.Cross(CameraFollowParent.up, Vector3.Cross(PlanarDirection, CameraFollowParent.up));
		//Quaternion planarRot = Quaternion.LookRotation(PlanarDirection, CameraFollowParent.up);

		//_targetVerticalAngle -= (lookInputVector.y * rotationPower);
		//_targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, minAngle, maxAngle);
		//Quaternion verticalRot = Quaternion.Euler(_targetVerticalAngle, 0, 0);
		//Quaternion targetRotation = Quaternion.Slerp(transform.rotation, planarRot * verticalRot, 1f - Mathf.Exp(-RotationSharpness * deltaTime));

		// Apply rotation
		//CameraFollowPoint.transform.rotation = targetRotation;
	}
}
