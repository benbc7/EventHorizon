using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATATAnimationController : MonoBehaviour {

	private Animator animator;
	private ATATController controller;

	private float lookDirection;
	private float moveDirection;
	private float speedPercent;

	private void Start () {
		animator = GetComponent<Animator> ();
		controller = GetComponentInParent<ATATController> ();
	}

	public void DeathAnimation () {
		animator.SetTrigger ("Die");
	}

	public void OnDeath () {
		GameManager.instance.OnBossDeath ();
	}

	public void SetDirections (float lookDirection, float velocityX) {
		this.lookDirection = lookDirection;
		moveDirection = Mathf.Sign (velocityX);
		speedPercent = velocityX / controller.moveSpeed;
		animator.SetFloat ("SpeedPercent", this.lookDirection * moveDirection * speedPercent);
	}

	public void OnSquash () {
		controller.CheckSquashCollision ();
	}

	public void StartDropAnimation () {
		animator.SetTrigger ("DroidDrop");
	}

	public void CreateDroids () {
		animator.SetTrigger ("CreateDroids");
	}

	public void SquashAnimation () {
		animator.SetTrigger ("Squash");
	}
}