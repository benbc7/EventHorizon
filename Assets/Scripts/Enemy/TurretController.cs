using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : Living, IActivatable {

	[Header ("Turret Controller")]
	public Projectile projectile;
	public float damage = 1f;
	public float spawnVelocity = 25f;
	public float msBetweenShots = 500f;
	public Transform weaponPivot;
	public Transform projectileSpawn;
	public GameObject explosionParticles;

	private float nextShotTime;
	private Transform player;
	private float velocity;
	private float smoothTime = 0.55f;

	public bool activateAtStart = false;
	private bool activated = false;

	public bool Activated {
		get {
			return activated;
		}
		set {
			activated = value;
		}
	}

	public void Activate () {
		activated = true;
		nextShotTime = Time.time + msBetweenShots / 1000f;
	}

	protected override void Die () {
		base.Die ();

		if (explosionParticles != null) {
			Instantiate (explosionParticles, transform.position, Quaternion.identity);
		}

		Destroy (gameObject);
	}

	protected override void Start () {
		base.Start ();
		player = GameObject.Find ("Player").transform;
		PoolManager.instance.CreatePool (projectile.gameObject, 30);
		PoolManager.instance.CreatePool (projectile.sparkParticle.gameObject, 15);
		if (activateAtStart) {
			Activate ();
		}
	}

	private void Update () {
		if (activated) {
			AimTurret ();
			if (Time.time > nextShotTime) {
				Shoot ();
			}
		}
	}

	private void AimTurret () {
		Vector2 direction = ((Vector2) player.position + Vector2.up * 0.5f - (Vector2) weaponPivot.position).normalized;
		float currentAngle = weaponPivot.localEulerAngles.z;
		float targetAngle = Vector2.Angle (Vector2.up, direction);
		bool rightSide = player.position.x > weaponPivot.position.x;
		targetAngle = (rightSide) ? 360 - targetAngle : targetAngle;
		float smoothedAngle = Mathf.SmoothDampAngle (currentAngle, targetAngle, ref velocity, smoothTime);

		weaponPivot.localEulerAngles = Vector3.forward * smoothedAngle;
	}

	private void Shoot () {
		nextShotTime = Time.time + msBetweenShots / 1000f;
		Projectile newProjectile = PoolManager.instance.ReuseObject (projectile.gameObject, projectileSpawn.position, projectileSpawn.rotation).GetComponent<Projectile> ();
		newProjectile.SetSpeed (spawnVelocity);
		newProjectile.SetDamage (damage);
		ParticleSystem particles = PoolManager.instance.ReuseObject (projectile.sparkParticle.gameObject, projectileSpawn.position, Quaternion.identity).GetComponent<ParticleSystem> ();
		particles.Play ();
		AudioManager.instance.PlaySound (projectile.shootSFX);
		CameraController.instance.CameraShake (0.1f);
	}
}