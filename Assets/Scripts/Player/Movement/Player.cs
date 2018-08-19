using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Player : Living {

	[Header ("Movement")]
	public float moveSpeed = 6;
	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = 0.4f;
	private float accelerationTimeAirborne = 0.25f;
	private float accelerationTimeGrounded = 0.15f;

	[Header ("Wall Sliding")]
	public Vector2 wallJumpClimb = new Vector2 (7.5f, 16f);
	public Vector2 wallJumpOff = new Vector2 (8.5f, 7f);
	public Vector2 wallLeap = new Vector2 (18, 17);
	public float wallSlideSpeedMax = 3;
	public float wallStickTime = 0.25f;
	private float timeToWallUnstick;

	[Header ("Other")]
	public Transform modelTransform;
	public Transform gunTransform;
	public Transform weaponAnchor;
	public Transform rightHandTarget;
	public Transform leftHandTarget;

	[Tooltip ("Angle at which the model\nbegins to rotate to look\nthe other direction")]
	public float modelRotationAngle = 10;
	private Transform projectileSpawn;

	private float gravity;
	private float maxJumpVelocity;
	private float minJumpVelocity;
	private Vector3 velocity;
	private float velocityXSmoothing;

	private Controller2D controller;
	private AnimationController animationController;

	private Vector2 directionalInput;
	private bool wallSliding;

	private bool paused;
	private bool dashing;
	private bool canDoubleJump;
	private int wallDirectionX;

	public void SetDashing (bool _dashing) {
		dashing = _dashing;
		animationController.SetDashing (dashing);
	}

	protected override void Start () {
		base.Start ();
		GameManager.instance.PauseEvent += Pause;
		controller = GetComponent<Controller2D> ();
		projectileSpawn = GetComponent<WeaponController> ().projectileSpawn;
		animationController = GetComponentInChildren<AnimationController> ();
		animationController.Initialize (rightHandTarget, leftHandTarget, projectileSpawn, controller);

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs (gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
	}

	private void Update () {
		if (!paused) {
			if (!dead) {
				CalculateVelocity ();
				HandleWallSliding ();

				controller.Move (velocity * Time.deltaTime, directionalInput);
				animationController.SetSpeedPercent (controller.collisions.mouseLookDirection, directionalInput.x);

				if (controller.collisions.above || controller.collisions.below) {
					canDoubleJump = false;
					if (controller.collisions.slidingDownMaxSlope) {
						velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
					} else {
						velocity.y = 0;
					}
				}
			} else {
				controller.Move (Vector2.up * gravity * Time.deltaTime);
			}
		}
	}

	protected override void Die () {
		base.Die ();
		animationController.DeathAnimation ();
	}

	public void Pause () {
		paused = !paused;
	}

	public void LookAt (Vector2 lookPoint) {
		if (!dead) {
			Vector2 direction = (lookPoint - (Vector2) transform.position).normalized;
			int sideMultiplier = (lookPoint.x < transform.position.x) ? 1 : -1;
			controller.collisions.mouseLookDirection = -sideMultiplier;
			float zAngle = Vector2.Angle (Vector2.up, direction);
			float yAngle;

			if (zAngle * sideMultiplier >= -modelRotationAngle && zAngle * sideMultiplier <= modelRotationAngle) {
				yAngle = Mathf.LerpAngle (0, 180, (zAngle * sideMultiplier + modelRotationAngle) / (modelRotationAngle * 2));
			} else {
				yAngle = (sideMultiplier < 0) ? 0 : 180;
			}

			weaponAnchor.localRotation = Quaternion.Euler (zAngle * Vector3.forward * sideMultiplier);
			gunTransform.localRotation = Quaternion.Euler ((yAngle - 90) * Vector3.up + 90 * Vector3.right);
			modelTransform.localRotation = Quaternion.Euler ((yAngle + 90) * Vector3.up);
		} else {
			modelTransform.localRotation = Quaternion.Euler (Vector3.up * 90);
		}
	}

	public void SetDirectionalInput (Vector2 input) {
		directionalInput = input;
	}

	public void OnDashInput (int faceDirection) {
		faceDirection = (faceDirection != 0) ? faceDirection : controller.collisions.mouseLookDirection;
		velocity.x = maxJumpVelocity * 0.9f * faceDirection;
		velocity.y = 0;
	}

	public void OnJumpInputDown () {
		if (wallSliding) {
			if (wallDirectionX == directionalInput.x) {
				velocity.x = -wallDirectionX * wallJumpClimb.x;
				velocity.y = wallJumpClimb.y;
			} else if (directionalInput.x == 0) {
				velocity.x = -wallDirectionX * wallJumpOff.x;
				velocity.y = wallJumpOff.y;
			} else {
				velocity.x = -wallDirectionX * wallLeap.x;
				velocity.y = wallLeap.y;
			}
			canDoubleJump = true;
			animationController.OnJumpTrigger ();
		}
		if (controller.collisions.below) {
			if (controller.collisions.slidingDownMaxSlope) {
				if (directionalInput.x != -Mathf.Sign (controller.collisions.slopeNormal.x)) { // not jumping against max slope
					velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
					velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
				}
			} else {
				velocity.y = maxJumpVelocity;
				canDoubleJump = true;
				animationController.OnJumpTrigger ();
			}
		} else if (canDoubleJump) {
			velocity.y = maxJumpVelocity;
			canDoubleJump = false;
			animationController.OnJumpTrigger ();
		}
	}

	public void OnJumpInputUp () {
		if (velocity.y > minJumpVelocity) {
			velocity.y = minJumpVelocity;
		}
	}

	private void HandleWallSliding () {
		wallDirectionX = (controller.collisions.left) ? -1 : 1;
		wallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
			wallSliding = true;

			if (velocity.y < -wallSlideSpeedMax) {
				velocity.y = -wallSlideSpeedMax;
			}

			if (timeToWallUnstick > 0) {
				velocityXSmoothing = 0;
				velocity.x = 0;

				if (directionalInput.x != wallDirectionX && directionalInput.x != 0) {
					timeToWallUnstick -= Time.deltaTime;
				} else {
					timeToWallUnstick = wallStickTime;
				}
			} else {
				timeToWallUnstick = wallStickTime;
			}
		}
	}

	private void CalculateVelocity () {
		float targetVelocityX = directionalInput.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
		if (!dashing) {
			velocity.y += gravity * Time.deltaTime;
		}
	}
}