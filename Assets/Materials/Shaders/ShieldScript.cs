using UnityEngine;
using System.Collections;

public class ShieldScript : MonoBehaviour {

	[Range (0.01f, 1.0f)] public float effectSpeed;

	private float effectTime = 0;
	new private Renderer renderer;
	private bool shieldActive = true;

	// Use this for initialization
	private void Start () {
		renderer = GetComponent<Renderer> ();
		renderer.sharedMaterial.SetFloat ("_EffectTime", effectTime);
	}

	private void FixedUpdate () {

		//if effect is going decriment it
		if (effectTime > 0) {
			effectTime -= Time.deltaTime * effectSpeed;

			//tell the shader the new effect time
			renderer.sharedMaterial.SetFloat ("_EffectTime", effectTime);
		}
	}

	private void OnTriggerEnter2D (Collider2D hit) {
		if (shieldActive) {

			//tell the shader the collision position
			renderer.sharedMaterial.SetVector ("_Position", transform.InverseTransformPoint (hit.transform.position));

			//reset the effect time
			effectTime = 1.0f;
		}
	}
}