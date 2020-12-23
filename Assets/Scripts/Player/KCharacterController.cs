﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using System;

public enum KCharacterState
{
	Default,
	Charging,
}

public enum KOrientationMethod
{
	TowardsCamera,
	TowardsMovement,
}

public enum KBonusOrientationMethod
{
	None,
	TowardsGravity,
	TowardsGroundSlopeAndGravity,
}

public class KCharacterController : MonoBehaviour, ICharacterController
{
	[SerializeField] public KPlayer kplayer;
	[SerializeField] public KinematicCharacterMotor Motor;
	[SerializeField] public Animator anim;
	[SerializeField] public List<Transform> GroundChecksList;
	[SerializeField] public LayerMask planetLayer;
	[Header("Stable Movement")]
	public bool RestrictMovement;
	public bool DefyGravity;
	public float MaxStableMoveSpeed = 10f;
	public float StableMovementSharpness = 15f;
	public float OrientationSharpness = 10f;
	public KOrientationMethod OrientationMethod = KOrientationMethod.TowardsCamera;

	[Header("Air Movement")]
	public float MaxAirMoveSpeed = 15f;
	public float AirAccelerationSpeed = 15f;
	public float Drag = 0.1f;

	[Header("Jumping")]
	public bool Jump;
	public bool AllowJumpingWhenSliding = false;
	public bool AllowDoubleJump = false;
	public bool AllowWallJump = false;
	public float JumpUpSpeed = 10f;
	public float JumpScalableForwardSpeed = 10f;
	public float JumpPreGroundingGraceTime = 0f;
	public float JumpPostGroundingGraceTime = 0f;

	[Header("Charging")]
	public float ChargeSpeed = 15f;
	public float MaxChargeTime = 1.5f;
	public float StoppedTime = 1f;
	private float DownwardForce = 0f;

	[Header("Misc")]
	public List<Collider> IgnoredColliders = new List<Collider>();
	public KBonusOrientationMethod BonusOrientationMethod = KBonusOrientationMethod.None;
	public float BonusOrientationSharpness = 10f;
	public Vector3 Gravity = new Vector3(0, -30f, 0);
	public Vector3 _Gravity;
	public float GravityStrength = 30;
	public Vector3 targetPosition;
	public KCharacterState CurrentCharacterState;
	public float HitAngle = 10f;
	public float HitLength = 2.5f;
	public bool TouchDownPlanet;
	public float SavedHit;
	public float PullDamping = 0.5f;
	public int RaycastCount = 18;
	private Collider[] _probedColliders = new Collider[8];
	private RaycastHit[] _probedHits = new RaycastHit[8];
	private Vector3 _moveInputVector;
	private Vector3 _lookInputVector;
	private bool _jumpRequested = false;
	private bool _jumpConsumed = false;
	private bool _doubleJumpConsumed = false;
	private bool _jumpedThisFrame = false;
	private bool _canWallJump = false;
	private Vector3 _wallJumpNormal;
	private float _timeSinceJumpRequested = Mathf.Infinity;
	private float _timeSinceLastAbleToJump = 0f;
	private Vector3 _internalVelocityAdd = Vector3.zero;
	private bool _shouldBeCrouching = false;
	private bool _isCrouching = false;

	private Vector3 _currentChargeVelocity;
	private bool _isStopped;
	private float _timeSinceStartedCharge = 0;
	private float _timeSinceStopped = 0;
	private Vector3 lastInnerNormal = Vector3.zero;
	private Vector3 lastOuterNormal = Vector3.zero;

	private void Awake()
	{
		TransitionToState(KCharacterState.Default);
		Motor.CharacterController = this;
	}

	private void Start()
	{
		for (int i = 0; i < RaycastCount; i++)
		{
			GameObject go = new GameObject();
			go.transform.parent = transform;
			go.transform.localPosition = new Vector3(0f, -1f, 0f);
			go.transform.localRotation = Quaternion.Euler(new Vector3(i * 360 / RaycastCount, 0f, 0f));
			GroundChecksList.Add(go.transform);
		}
	}
	private void Update()
	{
		anim.SetBool("IsGrounded", Motor.GroundingStatus.FoundAnyGround);
		SetInputs();
	}
	/// <summary>
	/// Handles movement state transitions and enter/exit callbacks
	/// </summary>
	public void TransitionToState(KCharacterState newState)
	{
		KCharacterState tmpInitialState = CurrentCharacterState;
		OnStateExit(tmpInitialState, newState);
		CurrentCharacterState = newState;
		OnStateEnter(newState, tmpInitialState);
	}

	public void PullAndDrop()
	{
		DefyGravity = true;
		RestrictMovement = true;
		Motor.BaseVelocity = Vector3.zero;
	}


	public void DashToTarget(Vector3 _destinationPosition, float _secondsToTarget, float _positionForwardOffset)
	{
		targetPosition = _destinationPosition;
		MaxChargeTime = _secondsToTarget;
		ChargeSpeed = _positionForwardOffset;
		DownwardForce = _positionForwardOffset;
		TransitionToState(KCharacterState.Charging);
	}

	/// <summary>
	/// Event when entering a state
	/// </summary>
	public void OnStateEnter(KCharacterState state, KCharacterState fromState)
	{
		switch (state)
		{
			case KCharacterState.Default:
				{
					break;
				}
			case KCharacterState.Charging:
				{
					//Motor.ForceUnground(0.1f);
					_currentChargeVelocity = (targetPosition - transform.position - Motor.CharacterUp * GravityStrength).normalized * ChargeSpeed;

					_isStopped = false;
					_timeSinceStartedCharge = 0f;
					_timeSinceStopped = 0f;
					break;
				}
		}
	}

	/// <summary>
	/// Event when exiting a state
	/// </summary>
	public void OnStateExit(KCharacterState state, KCharacterState toState)
	{
		switch (state)
		{
			case KCharacterState.Default:
				{
					break;
				}
		}
	}

	/// <summary>
	/// This is called every frame by ExamplePlayer in order to tell the character what its inputs are
	/// </summary>
	public void SetInputs()
	{
		// Clamp input
		Vector3 moveInputVector = Vector3.zero;
		if (kplayer)
			moveInputVector = Vector3.ClampMagnitude(new Vector3(kplayer.MovementStickValue.normalized.x, 0f, kplayer.MovementStickValue.normalized.y), 1f);
		if (RestrictMovement)
			moveInputVector = Vector3.zero;

		// Calculate camera direction and rotation on the character plane
		Quaternion finalRotation = Quaternion.identity;
		//if (kplayer)
			//finalRotation = kplayer.CameraFollowPoint.rotation;
		Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(finalRotation * Vector3.forward, Motor.CharacterUp).normalized;
		if (cameraPlanarDirection.sqrMagnitude == 0f)
		{
			cameraPlanarDirection = Vector3.ProjectOnPlane(finalRotation * Vector3.up, Motor.CharacterUp).normalized;
		}
		Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

		switch (CurrentCharacterState)
		{
			case KCharacterState.Default:
				{
					// Move and look inputs
					_moveInputVector = cameraPlanarRotation * moveInputVector;

					switch (OrientationMethod)
					{
						case KOrientationMethod.TowardsCamera:
							_lookInputVector = cameraPlanarDirection;
							break;
						case KOrientationMethod.TowardsMovement:
							_lookInputVector = _moveInputVector.normalized;
							break;
					}

					// Jumping input
					if (kplayer)
						Jump = kplayer.Jump;
					if (Jump)
					{
						_timeSinceJumpRequested = 0f;
						_jumpRequested = true;
					}

					// Crouching input
					//if (kplayer.CrouchDown)
					//{
					//	_shouldBeCrouching = true;

					//	if (!_isCrouching)
					//	{
					//		_isCrouching = true;
					//		Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
					//		MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
					//	}
					//}
					//else if (kplayer.CrouchUp)
					//{
					//	_shouldBeCrouching = false;
					//}

					break;
				}
		}
	}

	private Quaternion _tmpTransientRot;

	/// <summary>
	/// (Called by KinematicCharacterMotor during its update cycle)
	/// This is called before the character begins its movement update
	/// </summary>
	public void BeforeCharacterUpdate(float deltaTime)
	{
		switch (CurrentCharacterState)
		{
			case KCharacterState.Default:
				{
					break;
				}
			case KCharacterState.Charging:
				{
					// Update times
					_timeSinceStartedCharge += deltaTime;
					if (_isStopped)
					{
						_timeSinceStopped += deltaTime;
					}
					break;
				}
		}
	}

	/// <summary>
	/// (Called by KinematicCharacterMotor during its update cycle)
	/// This is where you tell your character what its rotation should be right now. 
	/// This is the ONLY place where you should set the character's rotation
	/// </summary>
	public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
	{
		switch (CurrentCharacterState)
		{
			case KCharacterState.Default:
				{
					if (_lookInputVector.sqrMagnitude > 0f && OrientationSharpness > 0f)
					{
						// Smoothly interpolate from current to target look direction
						Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

						// Set the current rotation (which will be used by the KinematicCharacterMotor)
						currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
					}

					Vector3 currentUp = (currentRotation * Vector3.up);
					if (BonusOrientationMethod == KBonusOrientationMethod.TowardsGravity)
					{
						// Rotate from current up to invert gravity
						Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
						currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
					}
					else if (BonusOrientationMethod == KBonusOrientationMethod.TowardsGroundSlopeAndGravity)
					{
						if (Motor.GroundingStatus.IsStableOnGround)
						{
							Vector3 initialCharacterBottomHemiCenter = Motor.TransientPosition + (currentUp * Motor.Capsule.radius);

							Vector3 smoothedGroundNormal = Vector3.Slerp(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
							currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) * currentRotation;

							// Move the position to create a rotation around the bottom hemi center instead of around the pivot
							Motor.SetTransientPosition(initialCharacterBottomHemiCenter + (currentRotation * Vector3.down * Motor.Capsule.radius));
						}
						else
						{
							Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
							currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
						}
					}
					else
					{
						Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
						currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
					}

					if (Motor.GroundingStatus.IsStableOnGround)
					{
						Vector3 initialCharacterBottomHemiCenter = Motor.TransientPosition + (currentUp * Motor.Capsule.radius);

						Vector3 smoothedGroundNormal = Vector3.Slerp(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
						transform.localRotation = Quaternion.Euler(initialCharacterBottomHemiCenter + (currentRotation * Vector3.down * Motor.Capsule.radius));
					}
					break;
				}
		}
	}

	/// <summary>
	/// (Called by KinematicCharacterMotor during its update cycle)
	/// This is where you tell your character what its velocity should be right now. 
	/// This is the ONLY place where you can set the character's velocity
	/// </summary>
	public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
	{
		_Gravity = Gravity;
		switch (CurrentCharacterState)
		{
			case KCharacterState.Default:
				{
					// Ground movement
					if (Motor.GroundingStatus.IsStableOnGround)
					{
						float currentVelocityMagnitude = currentVelocity.magnitude;
						anim.SetFloat("Run", currentVelocityMagnitude);

						Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;
						if (currentVelocityMagnitude > 0f && Motor.GroundingStatus.SnappingPrevented)
						{
							// Take the normal from where we're coming from
							Vector3 groundPointToCharacter = Motor.TransientPosition - Motor.GroundingStatus.GroundPoint;
							if (Vector3.Dot(currentVelocity, groundPointToCharacter) >= 0f)
							{
								effectiveGroundNormal = Motor.GroundingStatus.OuterGroundNormal;
							}
							else
							{
								effectiveGroundNormal = Motor.GroundingStatus.InnerGroundNormal;
							}
						}

						// Reorient velocity on slope
						currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

						// Calculate target velocity
						Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
						Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;
						Vector3 targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;

						// Smooth movement Velocity
						currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));
					}
					// Air movement
					else
					{
						// Add move input
						if (_moveInputVector.sqrMagnitude > 0f)
						{
							Vector3 addedVelocity = _moveInputVector * AirAccelerationSpeed * deltaTime;

							Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);

							// Limit air velocity from inputs
							if (currentVelocityOnInputsPlane.magnitude < MaxAirMoveSpeed)
							{
								// clamp addedVel to make total vel not exceed max vel on inputs plane
								Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, MaxAirMoveSpeed);
								addedVelocity = newTotal - currentVelocityOnInputsPlane;
							}
							else
							{
								// Make sure added vel doesn't go in the direction of the already-exceeding velocity
								if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
								{
									addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
								}
							}

							// Prevent air-climbing sloped walls
							if (Motor.GroundingStatus.FoundAnyGround)
							{
								if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
								{
									Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
									addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
								}
							}

							// Apply added velocity
							currentVelocity += addedVelocity;
						}

						// Gravity
						currentVelocity += _Gravity * deltaTime;

						// Drag
						currentVelocity *= (1f / (1f + (Drag * deltaTime)));
					}

					// Handle jumping
					_jumpedThisFrame = false;
					_timeSinceJumpRequested += deltaTime;
					if (_jumpRequested)
					{
						// See if we actually are allowed to jump
						if (!_jumpConsumed && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime))
						{
							// Calculate jump direction before ungrounding
							Vector3 jumpDirection = Motor.CharacterUp;
							if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
							{
								jumpDirection = Motor.GroundingStatus.GroundNormal;
							}

							// Makes the character skip ground probing/snapping on its next update. 
							// If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
							Motor.ForceUnground();

							// Add to the return velocity and reset jump state
							currentVelocity += (jumpDirection * JumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
							currentVelocity += (_moveInputVector * JumpScalableForwardSpeed);

							_jumpRequested = false;
							_jumpConsumed = true;
							_jumpedThisFrame = true;
						}
					}

					// Take into account additive velocity
					if (_internalVelocityAdd.sqrMagnitude > 0f)
					{
						currentVelocity += _internalVelocityAdd;
						_internalVelocityAdd = Vector3.zero;
					}
					if (RestrictMovement)
						currentVelocity = Vector3.zero;
				}
				break;
			case KCharacterState.Charging:
				{
					if (!_isStopped)
					{
						currentVelocity = _currentChargeVelocity;
					}
					// Gravity
					currentVelocity += Gravity * deltaTime;

					// Drag
					currentVelocity *= (1f / (1f + (Drag * deltaTime)));
				}
				break;
		}
		if (DefyGravity)
		{
			currentVelocity = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);
		}
	}

	/// <summary>
	/// (Called by KinematicCharacterMotor during its update cycle)
	/// This is called after the character has finished its movement update
	/// </summary>
	public void AfterCharacterUpdate(float deltaTime)
	{
		switch (CurrentCharacterState)
		{
			case KCharacterState.Default:
				{
					// Handle jump-related values
					{
						// Handle jumping pre-ground grace period
						if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
						{
							_jumpRequested = false;
						}

						if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
						{
							// If we're on a ground surface, reset jumping values
							if (!_jumpedThisFrame)
							{
								_jumpConsumed = false;
							}
							_timeSinceLastAbleToJump = 0f;
						}
						else
						{
							// Keep track of time since we were last able to jump (for grace period)
							_timeSinceLastAbleToJump += deltaTime;
						}
					}

					// Handle uncrouching
					if (_isCrouching && !_shouldBeCrouching)
					{
						// Do an overlap test with the character's standing height to see if there are any obstructions
						Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
						if (Motor.CharacterOverlap(
							Motor.TransientPosition,
							Motor.TransientRotation,
							_probedColliders,
							Motor.CollidableLayers,
							QueryTriggerInteraction.Ignore) > 0)
						{
							// If obstructions, just stick to crouching dimensions
							Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
						}
						else
						{
							// If no obstructions, uncrouch
							_isCrouching = false;
						}
					}
					break;
				}
			case KCharacterState.Charging:
				{
					// Detect being stopped by elapsed time
					if (!_isStopped && _timeSinceStartedCharge > MaxChargeTime)
					{
						_isStopped = true;
					}

					// Detect end of stopping phase and transition back to default movement state
					if (_timeSinceStopped > StoppedTime)
					{
						TransitionToState(KCharacterState.Default);
					}
					break;
				}
		}
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
		switch (CurrentCharacterState)
		{
			case KCharacterState.Default:
				{
					// We can wall jump only if we are not stable on ground and are moving against an obstruction
					if (AllowWallJump && !Motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable)
					{
						_canWallJump = true;
						_wallJumpNormal = hitNormal;
					}
					break;
				}
			case KCharacterState.Charging:
				{
					// Detect being stopped by obstructions
					if (!_isStopped && !hitStabilityReport.IsStable && Vector3.Dot(-hitNormal, _currentChargeVelocity.normalized) > 0.5f)
					{
						_isStopped = true;
					}
					break;
				}
		}
	}

	public void AddVelocity(Vector3 velocity)
	{
		switch (CurrentCharacterState)
		{
			case KCharacterState.Default:
				{
					_internalVelocityAdd += velocity;
					break;
				}
		}
	}

	public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
	{
	}

	protected void OnLanded()
	{
		if (kplayer)
			kplayer.Jump = false;
	}

	protected void OnLeaveStableGround()
	{
	}

	public void OnDiscreteCollisionDetected(Collider hitCollider)
	{
	}
}