using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnLocalX : MonoBehaviour {

	[Range (0.1f, 10f)]
	public float rotationMutliplier = 1f;

	private bool rotating;
	private float rotationDirection;

	private void Start () {
		rotationDirection = (Random.Range (0f, 1f) > 0) ? 1 : -1;
		StartCoroutine (RotateOnX (rotationDirection));
	}

	private IEnumerator RotateOnX (float direction) {
		rotating = true;
		float time = 0;

		while (rotating) {
			float zAngle = transform.localEulerAngles.z;
			float xAngle = Mathf.LerpAngle (0, 359, time) * rotationDirection;
			transform.localRotation = Quaternion.Euler (Vector3.forward * zAngle + Vector3.right * xAngle);

			time += Time.deltaTime * rotationMutliplier;

			yield return null;
		}
	}
}