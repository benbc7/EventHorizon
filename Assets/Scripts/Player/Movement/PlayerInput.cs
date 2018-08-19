using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour {

	private Player player;
	private Camera viewCamera;
	private CameraController cameraController;
	private WeaponController weaponController;

	private bool alive = true;
	private bool canDash = true;
	private bool stunned = false;

	private float dashTimer = -1;
	private float stunTimer = -1;

	private Vector2 directionalInput;

	// Use this for initialization
	private void Start () {
		player = GetComponent<Player> ();
		viewCamera = Camera.main;
		cameraController = viewCamera.GetComponent<CameraController> ();
		weaponController = GetComponent<WeaponController> ();
	}

	// Update is called once per frame
	private void Update () {
		UpdateTimers ();

		if (alive && !stunned) {
			MovementInput ();
			LookInput ();
			WeaponInput ();
		} else {
			player.SetDirectionalInput (Vector2.zero);
		}

		directionalInput = new Vector2 ();
	}

	private void MovementInput () {
		directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		player.SetDirectionalInput (directionalInput);

		if (canDash) {
			DashInput ();
		}

		if (Input.GetButtonDown ("Jump")) {
			player.OnJumpInputDown ();
		}
		if (Input.GetButtonUp ("Jump")) {
			player.OnJumpInputUp ();
		}
	}

	private void DashInput () {
		if (Input.GetKeyDown (KeyCode.LeftShift)) {
			canDash = false;
			dashTimer = Time.time + 0.2f;
			player.SetDashing (true);
			player.OnDashInput ((directionalInput.x != 0) ? (int) Mathf.Sign (directionalInput.x) : 0);
		}
	}

	private void LookInput () {
		Vector3 mouseWorldPosition = viewCamera.ScreenToWorldPoint (Input.mousePosition);
		player.LookAt (new Vector2 (mouseWorldPosition.x, mouseWorldPosition.y));
		cameraController.CalculateCameraLookAhead (mouseWorldPosition);
	}

	private void WeaponInput () {
		if (Input.GetMouseButton (0)) {
			weaponController.Shoot (true);
		}
		if (Input.GetMouseButton (1)) {
			weaponController.Shoot (false);
		}
	}

	public void StunPlayer (float stunDuration) {
		stunned = true;
		stunTimer = Time.time + stunDuration;
	}

	private void UpdateTimers () {
		if (dashTimer != -1) {
			if (dashTimer <= Time.time) {
				canDash = true;
				player.SetDashing (false);
				dashTimer = -1;
			}
		}
		if (stunTimer != -1) {
			if (stunTimer <= Time.time && stunned == true) {
				stunned = false;
				stunTimer = -1;
			}
		}
	}
}