using UnityEngine;

public enum GameLevel { Forest, Ice, Road, Farm }
[CreateAssetMenu(fileName = "Global Info", menuName = "Data/Global Info")]
public class GlobalInfoData : ScriptableObject
{
	[Header("Debugger")]
	[SerializeField]
	public bool Debugging;

	[Header("Game Data")]
	[SerializeField]
	public GameLevel gameLevel = GameLevel.Road;
	[SerializeField]
	public int roomPlayersCount = 4;
	[SerializeField]
	public string roomMusic = "Platform Action LOOP";

	[Header("Player Data")]
	public float currentHealth;
	public float maxHealth;

	[Header("Stable Movement")]

    [SerializeField]
    public float MaxStableMoveSpeed = 4;
    [SerializeField]
    public float MaxCrouchMoveSpeed = 1f;
    [SerializeField]
    public float StableMovementSharpness = 10;
    [SerializeField]
    public float OrientationSharpness = 20f;

    [Header("Air Movement")]

    [SerializeField]
    public float MaxAirMoveSpeed = 4f;
    [SerializeField]
    public float AirAccelerationSpeed = 20f;
    [SerializeField]
    public float Drag = 0f;

    [Header("Jumping")]

    [SerializeField]
    public bool AllowJumpingWhenSliding = false;
    [SerializeField]
    public float JumpUpSpeed = 15f;
    [SerializeField]
    public float JumpScalableForwardSpeed = 0f;
    [SerializeField]
    public float JumpPreGroundingGraceTime = 0.1f;
    [SerializeField]
    public float JumpPostGroundingGraceTime = 0.1f;

    [Header("Dodging")]
    [SerializeField]
    public float DodgeSpeedY = 4f;
	[SerializeField]
	public float DodgeSpeedX = 2.5f;

	[Header("Stats")]
	[SerializeField]
	public float Stamina = 5f;
	[SerializeField]
	public float InvincibilityLength = 5f;
	[SerializeField]
	public float MinThrowPower = 3f;
	[SerializeField]
	public float ThrowPower = 10f;
	[SerializeField]
	public float ThrowHeight = 3f;
	[SerializeField]
	public float maxEscapeTime = 5f;
	[SerializeField]
	public float maxWarmup = 2.0f;
	[SerializeField]
	public float maxDodgeCooldown = 2f;
	[SerializeField]
	public float maxSpecial = 10f;
	[SerializeField]
	public float plusSpecial = 10f;
	[SerializeField]
	public float initialForceMultiplier = 1.0f;
	[SerializeField]
	public float impactSpeed = 4.0f;
	[SerializeField]
	public float aimRadius = 0.5f;

	[Header("Powerups")]
	[SerializeField]
	public float ShieldLength = 10f;
	[SerializeField]
	public float SpeedLength = 10f;
	[SerializeField]
	public float SpeedBonus = 4f;
	[SerializeField]
	public float DizzinessLength = 5f;
	[SerializeField]
	public float PoisonLength = 5f;

	[Header("Player")]
	[SerializeField]
	public float cardCostMult = 2f;
	[SerializeField]
	public int upgradeCostMult = 50;
}
