using UnityEngine;
using System.Collections;

public class SwarmController : Living, IActivatable, IPoolable {

    [Header ("Swarm Fields")]
    [Range (0f, 2f)]
    public float alignmentWeight;

    [Range (0f, 2f)]
    public float cohesionWeight;

    [Range (0f, 2f)]
    public float separationWeight;

    [Range (0f, 2f)]
    public float targetWeight;
    public int neighborRange;
    public int speed;
    public LayerMask swarmLayerMask;
    public float damage;
    public bool activateAtStart = false;

    private Transform player;
    private Rigidbody2D rb;
    private Vector2 alVector;
    private Vector2 coVector;
    private Vector2 sepVector;
    private Collider2D [] neighbors;
    private Vector2 position;
    private float rand;

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
        Activated = true;
    }


    public void ResetObject () {
        SetDefaults ();
        rand = Random.Range (0.5f, 1.5f);
    }

    public void Destroy () {
        gameObject.SetActive (false);
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
        AudioManager.instance.PlaySound ("SmallExplosion");
        CameraController.instance.CameraShake (0.25f);
        Destroy (gameObject);
    }

    protected override void Start () {
        base.Start ();
        player = GameObject.Find ("Player").transform;
        rb = gameObject.GetComponent<Rigidbody2D> ();
        rand = Random.Range (0.5f, 1.5f);
        if (activateAtStart) {
            activated = true;
        }
    }

    private void FixedUpdate () {
        if (activated && player != null) {
            //set its velocity to the normalized vector
            rb.AddForce (CalculateFinalVelocity () * speed);

            //clear out the variables for the next round
            neighbors = new Collider2D [0];
        }
    }

    private Vector2 CalculateFinalVelocity () {
        position = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y);

        //find the enemy's neighbors
        neighbors = Physics2D.OverlapCircleAll (position, neighborRange, 1 << swarmLayerMask);

        //call the calculation vectors and get the vector value
        alVector = CalculateAlignment () * Random.Range (0f, 2f);
        coVector = CalculateCohesion () * Random.Range (0f, 2f);
        sepVector = CalculateSeparation () * Random.Range (0f, 2f);

        //calculate all the vectors together while multiplying them by their weight
        Vector2 finalVelocity = new Vector2 ();
        finalVelocity.x += alVector.x * alignmentWeight + coVector.x * cohesionWeight + sepVector.x * separationWeight + ((player.position.x - transform.position.x) * targetWeight) * rand;
        finalVelocity.y += alVector.y * alignmentWeight + coVector.y * cohesionWeight + sepVector.y * separationWeight + ((player.position.y - transform.position.y) * targetWeight) * rand;

        //normalize the vector to the speed of the enemy
        return finalVelocity.normalized;
    }

    private Vector2 CalculateAlignment () {
        Vector2 v = Vector2.zero;
        foreach (Collider2D agent in neighbors) {
            v.x += agent.GetComponent<Rigidbody2D> ().velocity.x;
            v.y += agent.GetComponent<Rigidbody2D> ().velocity.y;
        }
        if (neighbors.Length == 0) {
            return v;
        } else {
            v.x /= neighbors.Length;
            v.y /= neighbors.Length;
            v.Normalize ();
            return v;
        }
    }

    private Vector2 CalculateCohesion () {
        Vector2 v = Vector2.zero;
        foreach (Collider2D agent in neighbors) {
            v.x += agent.transform.position.x;
            v.y += agent.transform.position.y;
        }
        if (neighbors.Length == 0) {
            return v;
        } else {
            v.x /= neighbors.Length;
            v.y /= neighbors.Length;
            v = new Vector2 (v.x - gameObject.transform.position.x, v.y - gameObject.transform.position.y);
            v.Normalize ();
            return v;
        }
    }

    private Vector2 CalculateSeparation () {
        Vector2 v = Vector2.zero;
        foreach (Collider2D agent in neighbors) {
            v.x += agent.transform.position.x - gameObject.transform.position.x;
            v.y += agent.transform.position.y - gameObject.transform.position.y;
        }
        if (neighbors.Length == 0) {
            return v;
        } else {
            v.x /= neighbors.Length;
            v.y /= neighbors.Length;
            v.x *= -1;
            v.y *= -1;
            v.Normalize ();
            return v;
        }
    }
}