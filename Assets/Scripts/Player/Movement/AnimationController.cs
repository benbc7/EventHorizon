using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {

	public Transform spineBone;
	public Transform headBone;

	private Animator animator;
	private Controller2D controller;

	private Transform rightHandTarget;
	private Transform leftHandTarget;
	private Transform projectileSpawn;

	private bool dead;
	private bool grounded;
	private float speedPercent;

	private void Start () {
		animator = GetComponent<Animator> ();
	}

	public void OnDeath () {
		GameManager.instance.OnPlayerDeath ();
	}

	public void DeathAnimation () {
		dead = true;
		animator.SetTrigger ("Die");
	}

	public void PlayFootstep () {
		AudioManager.instance.PlaySound ("Footstep");
	}

	public void PlayJumpLanding () {
		AudioManager.instance.PlaySound ("JumpLanding");
	}

	public void SetDashing (bool dashing) {
		if (dashing) {
			animator.SetTrigger ("Dash");
			animator.SetBool ("Dashing", true);
		} else {
			animator.SetBool ("Dashing", false);
		}
	}

	public void OnJumpTrigger () {
		animator.SetTrigger ("Jump");
	}

	public void SetSpeedPercent (float mouseLookDirection, float walkDirection) {
		float speedMultiplier = (walkDirection == 0) ? 1f : 2f;
		animator.SetFloat ("speedPercent", walkDirection * mouseLookDirection);
		animator.SetFloat ("speedMultiplier", speedMultiplier);
	}

	public void Initialize (Transform _rightHandTarget, Transform _leftHandTarget, Transform _projectileSpawn, Controller2D _controller) {
		rightHandTarget = _rightHandTarget;
		leftHandTarget = _leftHandTarget;
		projectileSpawn = _projectileSpawn;
		controller = _controller;
		grounded = controller.collisions.below;
	}

	private void OnAnimatorIK (int layerIndex) {
		if (!dead) {
			if (rightHandTarget != null && leftHandTarget != null) {
				animator.SetIKPositionWeight (AvatarIKGoal.RightHand, 1);
				animator.SetIKPosition (AvatarIKGoal.RightHand, rightHandTarget.position);
				animator.SetIKPositionWeight (AvatarIKGoal.LeftHand, 1);
				animator.SetIKPosition (AvatarIKGoal.LeftHand, leftHandTarget.position);
				animator.SetIKRotationWeight (AvatarIKGoal.RightHand, 1);
				animator.SetIKRotation (AvatarIKGoal.RightHand, rightHandTarget.rotation);
				animator.SetIKRotationWeight (AvatarIKGoal.LeftHand, 1);
				animator.SetIKRotation (AvatarIKGoal.LeftHand, leftHandTarget.rotation);
			}
		} else {
			animator.SetIKPositionWeight (AvatarIKGoal.RightHand, 0);
			animator.SetIKPositionWeight (AvatarIKGoal.LeftHand, 0);
			animator.SetIKRotationWeight (AvatarIKGoal.RightHand, 0);
			animator.SetIKRotationWeight (AvatarIKGoal.LeftHand, 0);
		}
	}

	private void LateUpdate () {
		Vector3 lookPoint = projectileSpawn.position;
		Vector3 direction = (lookPoint - spineBone.position).normalized;

		//spineBone.localRotation = Quaternion.LookRotation (direction);

		direction = (lookPoint - headBone.position).normalized;
		headBone.rotation = Quaternion.LookRotation (direction);

		if (grounded != controller.collisions.below) {
			grounded = controller.collisions.below;
			animator.SetBool ("Grounded", grounded);
		}

		transform.localPosition = Vector3.up * -1f;
	}
}