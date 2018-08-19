using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public class WormController : Living, IPoolable, IActivatable {

	public enum WormType {
		Normal, Fireball, Charging
	}

	[Header ("Worm")]
	public WormType wormType;
	public float moveSpeed = 15f;
    public float damage = 5f;
	public float accelerationTime = 0.1f;
	public float rotationSmoothTime = 0.75f;
	public ParticleSystem dirtParticles;
	public BodySections[] bodySections;
    public bool activateAtStart = false;

	private Rigidbody2D rb;
	private Transform parent;
	private Transform player;

	private Vector2 accelerationVelocity;
	private float rotationVelocity;

    private bool activated = false;
    public bool Activated {
        get {
            return activated;
        } set {
            activated = value;
        }
    }

    public void Activate () {
        Activated = true;
    }

    public void Initialize (Transform player) {
        this.player = player;
    }

	protected override void Start () {
		base.Start ();
		rb = GetComponent<Rigidbody2D> ();
		player = GameObject.Find ("Player").transform;
		parent = transform.parent;
        if (activateAtStart) {
            activated = true;
        }
	}

	private void FixedUpdate () {
        if (player != null && activated) {
			FollowPlayer ();
			SetColliderOffsets ();
		}
	}

	private IEnumerator Surface () {
		yield return null;
	}

	private void FollowPlayer () {
		Vector2 direction = ((Vector2) player.position - (Vector2) transform.position).normalized;
		bool rightSide = player.position.x > transform.position.x;
		float currentAngle = transform.localEulerAngles.z;
		float targetAngle = Vector2.Angle (Vector2.up, direction);
		targetAngle = (rightSide) ? 360 - targetAngle : targetAngle;

		Vector2 targetPosition = (Vector2) transform.position + (Vector2) transform.up * moveSpeed * Time.fixedDeltaTime;
		rb.MoveRotation (Mathf.SmoothDampAngle (currentAngle, targetAngle, ref rotationVelocity, rotationSmoothTime));
		rb.MovePosition (Vector2.SmoothDamp (transform.position, targetPosition, ref accelerationVelocity, accelerationTime, moveSpeed * 2, Time.fixedDeltaTime));
	}

	private void SetColliderOffsets () {
		for (int i = 0; i < bodySections.Length; i++) {
			bodySections [i].SetColliderPosition (transform);
		}
	}

    private void OnCollisionEnter2D (Collision2D collision) {
        if (collision.transform.tag == "Player") {
            IDamagable damagableObject = collision.transform.GetComponent<IDamagable> ();
            if (damagableObject != null) {
                damagableObject.TakeDamage (damage);
            }
        }
    }

	protected override void Die () {
		base.Die ();
		AudioManager.instance.PlaySound ("LargeExplosion");
		CameraController.instance.CameraShake (1f);
		Destroy (parent.gameObject);
	}

	public void ResetObject () {
	}

	public void Destroy () {
		gameObject.SetActive (false);
	}

	[System.Serializable]
	public class BodySections {
		public Transform body;
		public CircleCollider2D collider;

		public void SetColliderPosition (Transform head) {
			Vector2 localPosition = head.InverseTransformPoint (body.position);
			Vector2 offset = localPosition;
			collider.offset = localPosition;
		}
	}
}