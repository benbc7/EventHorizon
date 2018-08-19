using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerMissile : MonoBehaviour {

	public float smoothTime = 0.85f;
	private float velocity;

	private Transform player;

	private void Start () {
		player = GameObject.Find ("Player").transform;
	}

	private void Update () {
		Vector2 direction = ((Vector2) player.position + Vector2.up - (Vector2) transform.position).normalized;
		bool rightSide = player.position.x > transform.position.x;
		float currentAngle = transform.localEulerAngles.z;
		float targetAngle = Vector2.Angle (Vector2.up, direction);
		targetAngle = (rightSide) ? 360 - targetAngle : targetAngle;
		float smoothedAngle = Mathf.SmoothDampAngle (currentAngle, targetAngle, ref velocity, smoothTime);
		transform.localEulerAngles = Vector3.forward * smoothedAngle;
	}
}
