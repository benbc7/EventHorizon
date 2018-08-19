using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {

	public static Vector2 XZ (this Vector3 v3) {
		return new Vector2 (v3.x, v3.z);
	}

	public static Vector3 RoundToInt (this Vector3 v3) {
		return new Vector3 (Mathf.RoundToInt (v3.x), Mathf.RoundToInt (v3.y), Mathf.RoundToInt (v3.z));
	}

	public static Vector3 ToVector3WithZ (this Vector2 v2, float z) {
		return new Vector3 (v2.x, v2.y, z);
	}

	public static float MapValue (this float referenceValue, float fromMin, float fromMax, float toMin, float toMax) {
		/* This function maps (converts) a Float value from one range to another */
		return toMin + (referenceValue - fromMin) * (toMax - toMin) / (fromMax - fromMin);
	}
}