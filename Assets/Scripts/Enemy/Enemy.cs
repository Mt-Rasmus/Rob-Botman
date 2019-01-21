using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour {

    private Collider2D collider;
    public Animator animator;
    private float HP;
    public AudioClip DeathSound;
    public AudioSource audiosource1;

    public Transform Target;
    private float speed = 4f;
    public float rotateSpeed = 200f;
    [HideInInspector] public Rigidbody2D rb;
    private GameObject thePlayer;
    private Player PlayerComp;
   // private SpriteRenderer sprite;

    // Use this for initialization
    void Start () {


        HP = 99.0f;
        collider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();

      //  sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        thePlayer = GameObject.FindGameObjectWithTag("Player");
        PlayerComp = thePlayer.GetComponent<Player>();
    }
	
	// Update is called once per frame
	void Update () {

        Target = GameObject.FindGameObjectWithTag("Player").transform;

        Vector2 direction = (Vector2)Target.position - rb.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.velocity = transform.up * speed;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Projectile")
        {
            HP -= PlayerComp.damage;

            if (HP <= 0)
            {
                PlayerComp.score += 25f;
                StartCoroutine(disableCollider());
                audiosource1.PlayOneShot(DeathSound);
                animator.SetBool("EnemyDead", true);
                //transform.gameObject.tag = "EnemyDead";
                Destroy(gameObject, 1.75f);
            }

        }
    }
    IEnumerator disableCollider()
    {
        yield return new WaitForSeconds(0.1f);
        collider.enabled = false;
    }
}
