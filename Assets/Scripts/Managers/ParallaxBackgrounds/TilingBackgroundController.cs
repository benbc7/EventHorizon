using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (SpriteRenderer))]
public class TilingBackgroundController : MonoBehaviour, IPoolable {

	public int offsetX = 2;
	public bool hasRightTile = false;
	public bool hasLeftTile = false;

	public bool reverseScale = false;

	private float spriteWidth = 0f;
	private Camera cam;
	private Transform t;

	private void Awake () {
		cam = Camera.main;
		t = transform;
	}

	private void Start () {
		SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
		spriteWidth = renderer.sprite.bounds.size.x;
	}

	private void LateUpdate () {
		if (!hasLeftTile || !hasRightTile) {
			float camXExtend = cam.orthographicSize * Screen.width / Screen.height;
			float edgeVisiblePositionRight = (t.position.x + spriteWidth / 2) - camXExtend;
			float edgeVisiblePositionLeft = (t.position.x - spriteWidth / 2) + camXExtend;

			if (cam.transform.position.x >= edgeVisiblePositionRight - offsetX && !hasRightTile) {
				MakeNewTile (1);
				hasRightTile = true;
			} else if (cam.transform.position.x <= edgeVisiblePositionLeft + offsetX && !hasLeftTile) {
				MakeNewTile (-1);
				hasLeftTile = true;
			}
		}
	}

	private void MakeNewTile (int rightOrLeft) {
		Vector3 targetPosition = new Vector3 (t.position.x + spriteWidth * rightOrLeft, t.position.y, t.position.z);
		Transform newTile = Instantiate (t, targetPosition, Quaternion.identity, t.parent);

		if (reverseScale) {
			newTile.localScale = new Vector3 (newTile.localScale.x * -1, newTile.localScale.y, newTile.localScale.z);
		}

		if (rightOrLeft > 0) {
			newTile.GetComponent<TilingBackgroundController> ().hasLeftTile = true;
		} else {
			newTile.GetComponent<TilingBackgroundController> ().hasRightTile = true;
		}
	}

	public void ResetObject () {
		hasLeftTile = false;
		hasRightTile = false;
	}

	public void Destroy () {
		gameObject.SetActive (false);
	}
}