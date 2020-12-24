using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;

public class KAnimator : MonoBehaviour
{
	[SerializeField]
	public Animator anim;
	[SerializeField]
	private KPlayer kplayer;
	[SerializeField]
	private KCharacterController kcc;

	private void Update()
    {
		if (!kplayer.pv.IsMine)
			return;
		//anim.SetBool("Dodging", kplayer.Dodge);
		//anim.SetFloat("DodgeAxis", kplayer.DodgeAxis);
		anim.SetBool("Crouching", kplayer.Crouch);
		anim.SetFloat("Movement", new Vector2(kcc.Motor.BaseVelocity.x, kcc.Motor.BaseVelocity.z).magnitude);
		anim.SetBool("IsGrounded", kcc.Motor.GroundingStatus.IsStableOnGround);
	}
}
