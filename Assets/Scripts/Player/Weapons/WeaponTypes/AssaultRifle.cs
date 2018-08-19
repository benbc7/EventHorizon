using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultRifle : Weapon {

	[Header ("Assault Rifle")]
	public float damage;

	public override void Shoot () {
		if (Time.time > nextShotTime && canShoot) {
			nextShotTime = Time.time + msBetweenShots / 1000f;
			Projectile newProjectile = PoolManager.instance.ReuseObject (projectile.gameObject, projectileSpawn.position, projectileSpawn.rotation).GetComponent<Projectile> ();
			newProjectile.SetSpeed (spawnVelocity);
			newProjectile.SetDamage (damage);
			coolDown += Time.fixedDeltaTime * 150f;
			ParticleSystem particles = PoolManager.instance.ReuseObject (projectile.sparkParticle.gameObject, projectileSpawn.position, Quaternion.identity).GetComponent<ParticleSystem> ();
			particles.Play ();
			AudioManager.instance.PlaySound ("ShootAssaultRifle");
			CameraController.instance.CameraShake (0.1f);
		}
	}
}