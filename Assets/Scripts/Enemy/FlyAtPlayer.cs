using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAtPlayer : MonoBehaviour {

    public float moveSpeed = 15f;
    public float accelerationTime = 0.1f;
    public float rotationSmoothTime = 0.75f;

	private Rigidbody2D rb;
	private Transform player;

	private Vector2 accelerationVelocity;
	private float rotationVelocity;

	private void Start () {
		rb = GetComponent<Rigidbody2D> ();
		player = GameObject.Find ("Player").transform;
	}

	private void FixedUpdate () {
		Vector2 direction = ((Vector2) player.position - (Vector2) transform.position).normalized;
		bool rightSide = player.position.x > transform.position.x;
        float currentAngle = transform.localEulerAngles.z;
        float targetAngle = Vector2.Angle (Vector2.up, direction);
        targetAngle = (rightSide) ? 360 - targetAngle : targetAngle;

        Vector2 targetPosition = (Vector2) transform.position + (Vector2) transform.up * moveSpeed * Time.fixedDeltaTime;
		rb.MoveRotation (Mathf.SmoothDampAngle (currentAngle, targetAngle, ref rotationVelocity, rotationSmoothTime));
		rb.MovePosition (Vector2.SmoothDamp (transform.position, targetPosition, ref accelerationVelocity, accelerationTime, moveSpeed, Time.fixedDeltaTime));
	}
}