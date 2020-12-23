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
	public CharacterModel characterModel;
	public GameObject PlayerCharacter;
	public int skinNumber;
	public KCharacterController kcc;
	public KAnimator kanim;
	public KPickup kpickup;
	public ABC_StateManager ABCEvents;
	public ABC_Controller ABCcontroller;
	public CinemachineVirtualCamera cvm;
	private KInputActions controls;
	public Vector2 MovementStickValue;
	public bool Jump;
	public bool Crouch;

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
		controls = new KInputActions();
		controls.Player.Move.performed += context =>
		{
			MovementStickValue = context.ReadValue<Vector2>();
		};
		controls.Player.Jump.performed += context =>
		{
			var button = context.control as ButtonControl;
			if (button.wasPressedThisFrame)
				Jump = true;
			if (button.wasReleasedThisFrame)
				Jump = false;
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

	public void ActionPressed()
	{
		Crouch = true;
	}

	public void ActionReleased()
	{
		Crouch = false;
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
}
