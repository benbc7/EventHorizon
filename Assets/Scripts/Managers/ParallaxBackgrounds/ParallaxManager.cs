using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour {

	public Transform[] backgrounds;

	[Range (0.1f, 10f)]
	public float smoothing = 1f;

	private float[] parallaxScales;
	private Transform cameraTransform;
	private Vector3 previousCameraPosition;

	private void Awake () {
		cameraTransform = Camera.main.transform;
	}

	private void Start () {
		if (backgrounds != null) {
			previousCameraPosition = cameraTransform.position;
			parallaxScales = new float [backgrounds.Length];
			for (int i = 0; i < backgrounds.Length; i++) {
				parallaxScales [i] = backgrounds [i].position.z * -1;
			}
		}
	}

	private void Update () {
		if (backgrounds != null) {
			for (int i = 0; i < backgrounds.Length; i++) {
				float parallax = (previousCameraPosition.x - cameraTransform.position.x) * parallaxScales [i];
				float targetX = backgrounds [i].position.x + parallax;
				Vector3 targetPosition = new Vector3 (targetX, backgrounds [i].position.y, backgrounds [i].position.z);
				backgrounds [i].position = Vector3.Lerp (backgrounds [i].position, targetPosition, smoothing * Time.deltaTime);
			}

			previousCameraPosition = cameraTransform.position;
		}
	}
}