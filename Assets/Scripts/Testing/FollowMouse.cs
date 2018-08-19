using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour {

	private Camera viewCamera;
	private Rigidbody2D rb;
	private float speed = 8;

	private float accelerationTime = 0.1f;
	private Vector2 accelerationVelocity;
	private float rotationSmoothTime = 0.2f;
	private float rotationVelocity;

	private void Start () {
		viewCamera = Camera.main;
		rb = GetComponent<Rigidbody2D> ();
	}

	private void Update () {
		if (Input.GetKeyDown (KeyCode.W)) {
			speed++;
		} else if (Input.GetKeyDown (KeyCode.S)) {
			speed--;
		}
	}

	private void FixedUpdate () {
		Vector2 mouseWorldPosition = viewCamera.ScreenToWorldPoint (Input.mousePosition);
		Vector2 direction = (mouseWorldPosition - (Vector2) transform.position).normalized;
		int sideMultiplier = (mouseWorldPosition.y < transform.position.y) ? 1 : -1;
		float targetAngle = Vector2.Angle (Vector2.left, direction);
		Vector2 targetPosition = (Vector2) transform.position + direction * speed * Time.fixedDeltaTime;

		rb.MoveRotation (targetAngle * sideMultiplier);
		print (transform.localEulerAngles.x);
		rb.MovePosition (Vector2.SmoothDamp ((Vector2) transform.position, targetPosition, ref accelerationVelocity, accelerationTime, speed, Time.fixedDeltaTime));
	}
}