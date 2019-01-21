using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Ball2 : MonoBehaviour
{
    public GameObject ballPrefab;
    public float releaseTime;
    public float maxDragDistance;
    public Animator animator;
    public Animator animSlingGlow;

    public AudioClip buzz;
    public AudioClip explosionHit;
    public AudioClip explosionMiss;
    public AudioSource audiosource1;
    public AudioSource audiosource2;
    public GameObject menu;

    SpringJoint2D Spring_newBall;

    [HideInInspector]
    public bool isPressed = false;
    private SpringJoint2D spring;
    [HideInInspector]
    public Vector2 mousePos;
    [HideInInspector]
    public Vector2 mousePos0;
    Vector3 newRbPos;
    Vector2 rbPos0;
    Rigidbody2D newBall2;
    private GameObject Slingshot;
    private GameObject SlingGlow;
    GameObject spawnedBall;
    public float damping;
    private float minDamping;
    private Collider2D ballCollider;
    private bool buzzPlaying = false;
    private GameObject thePlayer;
    private Player PlayerComp;
    private float countDown = 0.0f;
    private bool released = false;
    private GameObject[] balls;

    void Start()
    {
        Slingshot = GameObject.FindGameObjectWithTag("SlingShot");
        SlingGlow = GameObject.FindGameObjectWithTag("SlingGlow");
        thePlayer = GameObject.FindGameObjectWithTag("Player");
        PlayerComp = thePlayer.GetComponent<Player>();

        releaseTime = .22f;
        maxDragDistance = 3.5f;
        newRbPos = new Vector3(0.0f, 0.0f, 0.0f);
        damping = 0.4f;
        minDamping = 0.02f;


        //  animator.enabled = true;
    }

    void Update()
    {
        if (released)
            countDown -= Time.deltaTime;

        PlayerComp = thePlayer.GetComponent<Player>();

        balls = GameObject.FindGameObjectsWithTag("Projectile");

        if(balls != null)
        {
            foreach (GameObject g in balls.ToList())
            {
                ballCollider = g.GetComponent<Collider2D>();
                // Destroy ball if it touching certain type of object (identify by layer)
                if (g && ballCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && !PlayerComp.BallPowerUP)
                {
                    animator = g.GetComponent<Animator>();
                    animator.SetBool("BallDead", true);
                    ballCollider = g.GetComponent<Collider2D>();
                    g.GetComponent<Collider2D>().enabled = false;

                    g.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
                    g.GetComponent<Rigidbody2D>().gravityScale = 0f;
                    audiosource1.PlayOneShot(explosionMiss);
                    Destroy(g, 0.15f);
                }
                else if (g && ballCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
                {
                    animator = g.GetComponent<Animator>();
                    animator.SetBool("BallDead", true);
                    ballCollider = g.GetComponent<Collider2D>();
                    g.GetComponent<Collider2D>().enabled = false;

                    g.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
                    g.GetComponent<Rigidbody2D>().gravityScale = 0f;

                    audiosource1.PlayOneShot(explosionHit);
                    Destroy(g, 0.15f);
                }
            }
        }

        if (isPressed)
        {
            
            if (!audiosource2.isPlaying)
            {
                audiosource2.Play();
            }

            buzzPlaying = true;

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            newRbPos = rbPos0 + (mousePos - mousePos0);

            // Set ball position while pressing down mouse key
            if(spawnedBall) { 

                if (Vector3.Distance(newRbPos, Slingshot.transform.position) > maxDragDistance)
                {
                    spawnedBall.transform.position = Slingshot.transform.position + (newRbPos - Slingshot.transform.position).normalized * maxDragDistance;
                }
                else
                    spawnedBall.transform.position = newRbPos;
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {

            if(countDown <= 0 && !PlayerComp.PlayerDead && !menu.activeSelf) // if countdown finished, another ball can be instantiated
            {
                // Instantiate ball and set initial properties
                released = false;
                spawnedBall = Instantiate(ballPrefab, Slingshot.transform.position, transform.rotation);
                animator = spawnedBall.GetComponent<Animator>();
                animSlingGlow = SlingGlow.GetComponent<Animator>();
                ballCollider = spawnedBall.GetComponent<Collider2D>();
                spawnedBall.GetComponent<Collider2D>().enabled = false;

                animSlingGlow.SetBool("Spawn", true);

                animSlingGlow.SetTrigger("triggerGlow");

                spawnedBall.GetComponent<SpringJoint2D>().connectedBody = Slingshot.GetComponent<Rigidbody2D>();
                spring = spawnedBall.GetComponent<SpringJoint2D>();
                spring.enabled = true;

                rbPos0 = spawnedBall.transform.position;
                mousePos0 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                spawnedBall.GetComponent<Rigidbody2D>().freezeRotation = false;
                spring.distance = 0.00000002f;
                spring.dampingRatio = damping;
                spring.frequency = 1.5f;
                isPressed = true;
                spawnedBall.GetComponent<Rigidbody2D>().isKinematic = true;

            }
        }

        if (spawnedBall && Input.GetMouseButton(0) && spring.dampingRatio > minDamping) // Decrease damping the longer one holds off firing the ball
        {
            spring.dampingRatio -= 0.003f;
        }

        if (spawnedBall && Input.GetMouseButton(0) && animator != null && animator.isActiveAndEnabled && spring.dampingRatio > minDamping) {
            animator.speed += 1.5f*Mathf.Pow((float)(animator.speed*0.05125), 2F);
        }

        if (Input.GetButtonUp("Fire1"))
        {
            // The ball is let go
            if(animSlingGlow && spawnedBall && countDown <= 0 && !PlayerComp.PlayerDead && !menu.activeSelf)
            {
                released = true;
                countDown = 0.3f;
                buzzPlaying = false;
                animSlingGlow.SetBool("Spawn", false);
                animSlingGlow.SetTrigger("triggerFade");
                animSlingGlow.SetTrigger("triggerStandby");
                isPressed = false;
                spawnedBall.GetComponent<Rigidbody2D>().isKinematic = false;
                StartCoroutine(Release());
            }

        }

        if(buzzPlaying == false && audiosource2.isPlaying)
        {
            if (audiosource2.volume > 0)
                audiosource2.volume = audiosource2.volume - .1f;
            else
            {
                audiosource2.Stop();
                audiosource2.volume = 1f;
            }
                
        }

    }

    IEnumerator Release()
    {
        yield return new WaitForSeconds(releaseTime); // must have IEnumerator for this (coroutine)

        if(spawnedBall)
        {
            spawnedBall.GetComponent<Collider2D>().enabled = true;
            spring.enabled = false;
            spring.dampingRatio = 0.9f;

            if(animator)
                animator.speed = 1f;

            Destroy(spawnedBall, 3f);
        }

    }

}
