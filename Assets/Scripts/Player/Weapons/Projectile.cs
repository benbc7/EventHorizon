/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios
Product:
Date:
*************************************************/

using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolable {

	public LayerMask collisionMask;
	public ParticleSystem sparkParticle;
	public string shootSFX;
	public string destroySFX;

	private float speed = 10f;
	private float damage = 1f;
	private float spawnTime;

	public void SetSpeed (float newSpeed) {
		speed = newSpeed;
	}

	public void SetDamage (float newDamage) {
		damage = newDamage;
	}

	private void Start () {
		spawnTime = Time.time;
	}

	public void ResetObject () {
		spawnTime = Time.time;
		if (shootSFX != null) {
			AudioManager.instance.PlaySound (shootSFX);
		}
	}

	private void Update () {
		if (Time.time >= spawnTime + 2f) {
			Destroy ();
		}
		float moveDistance = speed * Time.deltaTime;
		CheckCollisions (moveDistance);
		transform.Translate (Vector3.up * moveDistance);
	}

	private void CheckCollisions (float moveDistance) {
		RaycastHit2D hit = Physics2D.Raycast (transform.position, transform.up, moveDistance, collisionMask);
		if (hit) {
			OnHitObject (hit);
		}
	}

	private void OnHitObject (RaycastHit2D hit) {
		IDamagable damagableObject = hit.collider.GetComponent<IDamagable> ();
		if (damagableObject != null) {
			damagableObject.TakeDamage (damage);
		}

		int layer = hit.transform.gameObject.layer;
		if (layer == 8 || layer == 12) {
			AudioManager.instance.PlaySound ("SmallHit");
		} else if (layer == 11 || layer == 13 || layer == 15) {
			AudioManager.instance.PlaySound ("HardHit");
		}

		Destroy ();
	}

	public void Destroy () {
		if (gameObject.activeInHierarchy) {
			if (sparkParticle != null) {
				ParticleSystem particles = PoolManager.instance.ReuseObject (sparkParticle.gameObject, transform.position, Quaternion.identity).GetComponent<ParticleSystem> ();
				particles.Play ();
			}
			if (destroySFX != null) {
				AudioManager.instance.PlaySound (destroySFX);
			}
			gameObject.SetActive (false);
		}
	}

	public void DestroyAfter (float waitTime) {
		StartCoroutine (WaitToDestroy (waitTime));
	}

	private IEnumerator WaitToDestroy (float waitTime) {
		yield return new WaitForSeconds (waitTime);
		Destroy ();
	}
}