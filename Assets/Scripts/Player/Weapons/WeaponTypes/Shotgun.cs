using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon {

	[Header ("Shotgun")]
	public int numberOfProjectiles;

	[Range (5f, 120f)]
	public float spreadAngle;

	[Tooltip ("0 = infinity")]
	public float timeToSelfDestroy;

	public float fullSpreadDamage;

	public override void Shoot () {
		if (Time.time > nextShotTime && canShoot) {
			nextShotTime = Time.time + msBetweenShots / 1000f;
			ParticleSystem particles = PoolManager.instance.ReuseObject (projectile.sparkParticle.gameObject, projectileSpawn.position, Quaternion.identity).GetComponent<ParticleSystem> ();
			particles.Play ();
			AudioManager.instance.PlaySound ("ShootShotgun");
			CameraController.instance.CameraShake (1f);

			int sideMultiplier = (projectileSpawn.eulerAngles.y >= 90) ? -1 : 1;
			for (int i = 0; i < numberOfProjectiles; i++) {
				Projectile newProjectile = PoolManager.instance.ReuseObject (projectile.gameObject, projectileSpawn.position, projectileSpawn.rotation).GetComponent<Projectile> ();
				newProjectile.transform.rotation = Quaternion.Euler (Vector3.forward * sideMultiplier * (Random.Range (-spreadAngle, spreadAngle) + newProjectile.transform.eulerAngles.z));
				newProjectile.SetSpeed (Random.Range (spawnVelocity * 0.8f, spawnVelocity * 1.2f));
				newProjectile.SetDamage (fullSpreadDamage / numberOfProjectiles);
				newProjectile.DestroyAfter (timeToSelfDestroy);
				coolDown += Time.fixedDeltaTime * 50f;
			}
		}
	}

	protected override void OnValidate () {
		base.OnValidate ();
		if (numberOfProjectiles < 1) {
			numberOfProjectiles = 1;
		}
		if (timeToSelfDestroy < 0) {
			timeToSelfDestroy = 0;
		}
	}
}