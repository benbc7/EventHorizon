using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATATController : MonoBehaviour, IActivatable {

	public float moveSpeed;
	public Transform weaponPivot;
	public Transform projectileSpawn;
	public Projectile projectilePrefab;
	public float spawnVelocity = 25f;
	public GameObject droidPrefab;
	public float squashRange = 1f;
	public LayerMask playerMask;
	public LayerMask environmentMask;
	public float[] waypoints = new float[2];
	public GameObject healthBar;

	private int currentWaypoint = 0;

	[SerializeField, HideInInspector]
	private Vector3[] waypointPositions = new Vector3[2];

	[Header ("Delays"), Range (1, 1000)]
	public float msBetweenShots = 1000f;
	public float msBeforeSquash = 500f;
	public float msBeforeDroidDrop = 7000f;

	[Header ("Damage")]
	public float projectileDamage;
	public float squashDamage;
	public float numberOfDroidsToSpawn;

	private float smoothTime = 0.25f;
	private float launcherVelocity;

	private bool dead;
	private GameObject weakPoint;
	private ATATAnimationController animator;
	private Transform player;
	private bool canShoot = true;
	private bool droppingDroids;
	private bool walking;
	private float nextShotTime;
	private float nextSquashTime;
	private float nextDropTime;

	public bool CanShoot {
		get {
			return canShoot;
		}
		set {
			canShoot = value;
		}
	}

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
		healthBar.SetActive (true);
	}

	public void OnDeath () {
		dead = true;
	}

	private void Start () {
		healthBar.SetActive (false);
		animator = GetComponentInChildren<ATATAnimationController> ();
		player = GameObject.Find ("Player").transform;
		weakPoint = GetComponentInChildren<CircleCollider2D> ().gameObject;
		weakPoint.SetActive (false);
		PoolManager.instance.CreatePool (projectilePrefab.gameObject, 5);
		PoolManager.instance.CreatePool (droidPrefab, 20);
		nextDropTime = Time.time + msBeforeDroidDrop / 1000f;
	}

	private void Update () {
		if (activated && player != null && !dead) {
			AimRocketLaunchers ();

			if (canShoot) {
				if (Time.time > nextShotTime) {
					Shoot ();
				}

				CheckIfPlayerSquashable ();
			}

			if (!droppingDroids && Time.time > nextDropTime) {
				StartCoroutine (DroidDrop ());
			}
		}
	}

	private IEnumerator DroidDrop () {
		droppingDroids = true;
		canShoot = false;
		animator.StartDropAnimation ();
		weakPoint.SetActive (true);

		yield return new WaitForSeconds (4f);

		animator.CreateDroids ();
		StartCoroutine (SetCanShootAfter (true, 1.2f));

		for (int i = 0; i < numberOfDroidsToSpawn; i++) {
			SwarmController droid = PoolManager.instance.ReuseObject (droidPrefab, weakPoint.transform.position, Quaternion.identity).GetComponent<SwarmController> ();
			droid.Activate ();
			droid.startingHealth = 2.5f;
			droid.SetDefaults ();
			yield return new WaitForSeconds (0.15f);
		}

		yield return new WaitForSeconds (1.2f - (0.15f * numberOfDroidsToSpawn));

		weakPoint.SetActive (false);
		nextDropTime = Time.time + msBeforeDroidDrop / 1000f;
		droppingDroids = false;
	}

	private void AimRocketLaunchers () {
		Vector2 direction = ((Vector2) player.position + Vector2.up - (Vector2) weaponPivot.position).normalized;
		bool topSide = player.position.y + 1f > weaponPivot.position.y;
		float currentAngle = weaponPivot.localEulerAngles.z;
		float targetAngle = Vector2.Angle (Vector2.left, direction);
		targetAngle = (topSide) ? 360 - targetAngle : targetAngle;
		float smoothedAngle = Mathf.SmoothDampAngle (currentAngle, targetAngle, ref launcherVelocity, smoothTime);

		weaponPivot.localEulerAngles = Vector3.forward * smoothedAngle;
	}

	public void SetCanShoot (bool canShoot) {
		CanShoot = canShoot;
	}

	private void Shoot () {
		Projectile newProjectile = PoolManager.instance.ReuseObject (projectilePrefab.gameObject, projectileSpawn.position, projectileSpawn.rotation).GetComponent<Projectile> ();
		newProjectile.SetSpeed (spawnVelocity);
		nextShotTime = Time.time + msBetweenShots / 1000f;

		//ParticleSystem particles = PoolManager.instance.ReuseObject (projectilePrefab.sparkParticle.gameObject, projectileSpawn.position, projectileSpawn.rotation).GetComponent<ParticleSystem> ();
		//particles.Play ();
		CameraController.instance.CameraShake (1f);
	}

	private void Squash () {
		nextSquashTime = float.MaxValue;
		canShoot = false;
		animator.SquashAnimation ();
		StartCoroutine (SetCanShootAfter (true, 3.2f));
	}

	public void CheckSquashCollision () {
		Vector2 size = new Vector2 (squashRange, 1f);
		if (Physics2D.OverlapBox (transform.position + Vector3.up, size, 0f, playerMask)) {
			player.GetComponent<Living> ().TakeDamage (squashDamage);
		}
	}

	private IEnumerator SetCanShootAfter (bool _canShoot, float time) {
		yield return new WaitForSeconds (time);
		canShoot = _canShoot;
	}

	private void CheckIfPlayerSquashable () {
		if (Time.time > nextSquashTime) {
			Squash ();
		} else {
			Vector2 size = new Vector2 (squashRange, 1f);
			if (Physics2D.OverlapBox (transform.position + Vector3.up, size, 0f, playerMask)) {
				if (nextSquashTime == float.MaxValue) {
					nextSquashTime = Time.time + msBeforeSquash / 1000f;
				}
			} else {
				nextSquashTime = float.MaxValue;
			}
		}
	}

	private void OnDrawGizmosSelected () {
		Gizmos.color = Color.red;
		Vector3 size = new Vector3 (squashRange, 1f, 0.25f);
		Gizmos.DrawWireCube (transform.position + Vector3.up, size);

		Gizmos.color = Color.cyan;
		for (int i = 0; i < waypointPositions.Length; i++) {
			Gizmos.DrawWireSphere (waypointPositions [i], 0.25f);
		}
	}

	private void OnValidate () {
		for (int i = 0; i < waypoints.Length; i++) {

			if (waypointPositions.Length != waypoints.Length) {
				waypointPositions = new Vector3 [waypoints.Length];
			}

			if (waypoints [i] != waypointPositions [i].x) {
				Vector2 origin = new Vector2 (waypoints [i], 10f);
				RaycastHit2D hit = Physics2D.Raycast (origin, Vector2.down, 30f, environmentMask);
				if (hit) {
					waypointPositions [i] = new Vector3 (waypoints [i], hit.point.y, 0f);
				} else {
					waypointPositions [i] = new Vector3 (waypoints [i], 0f, 0f);
				}
			}
		}
	}
}