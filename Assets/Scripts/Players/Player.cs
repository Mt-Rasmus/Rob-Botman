using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

/*
 * Player controller taken in large part from this tutorial:
 * https://github.com/SebLague/2DPlatformer-Tutorial
 * https://www.youtube.com/playlist?list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz
 */

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    public float jumpHeight;
    public float timeToJumpApex;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6f;
    public float HP;
    public float damage = 50;
    public Animator animator;
    public AudioClip JumpSound;
    public AudioClip WalljumoSound;
    public AudioClip PlayerDeadSound;
    public AudioClip DamageTaken;
    public AudioSource audiosource1;
    public GameObject restartScreen;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .15f;
    public GameObject HealthBar;
    public bool BallPowerUP = false;
    float timeToWallUnstick;
    public float baseHP;
    public float score;

    private float gravity;
    private float jumpVelocity;
    public bool PlayerDead;
    Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;

    [HideInInspector]
    public Vector2 directionalInput;
    bool wallSliding;
    int wallDirX;

    // respawn
    public LevelManager gameLevelManager;
    public Vector3 respawnPoint;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        respawnPoint = transform.position;
        baseHP = 200;
        HP = 200f;
        score = 0f;
        animator = GetComponent<Animator>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    void Update()
    {
        CalculateVelocity();
        HandleWallSliding();

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        } 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "FallDetector")
        {
            Debug.Log(collision.gameObject.name + " : " + gameObject.name + " : " + Time.time);
            gameLevelManager.Respawn();
        }
        if (collision.tag == "Checkpoint")
        {
            respawnPoint = collision.transform.position;
        }

        if(collision.tag == "Projectile") // Extract projectile info from ball, and apply damage tp player
        {
            
            Ball2 BallComp = GetComponent<Ball2>();

            float damage = BallComp.mousePos0.x;

        }

        if(collision.tag == "Enemy" && PlayerDead == false)
        {
            float damage = 50f;

            HP -= damage;
            DecreseHealthBar(damage);
            audiosource1.PlayOneShot(DamageTaken);

            if (HP <= 0)
            {
                PlayerDead = true;          
                audiosource1.PlayOneShot(PlayerDeadSound);
                controller.enabled = false;                
                moveSpeed = 0;
                animator.SetBool("PlayerDead", true);
                restartScreen.SetActive(true);
            }
        }

    }

    public void DecreseHealthBar(float damage)
    {

        HealthBar = GameObject.FindGameObjectWithTag("HealthBar");

        foreach (Transform t in HealthBar.transform)
        {
            if (t.name == "Canvas")
            {
                foreach (Transform c in t.transform)
                {
                    if (c.name == "PowerUpBar")
                    {
                        foreach (Transform l in c.transform)
                        {
                            if (l.name == "LoadingBar")
                            {
                                if (l != null)
                                {
                                    l.GetComponent<Image>().fillAmount -= damage / baseHP;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (wallSliding)
        {
            audiosource1.PlayOneShot(WalljumoSound);
            if (wallDirX == directionalInput.x)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }
        if (controller.collisions.below)
        {
            audiosource1.PlayOneShot(JumpSound);
            velocity.y = jumpVelocity;
        }
    }

    void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;

        if ((controller.collisions.left || controller.collisions.right) &&
            !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;
            if (velocity.y < -wallSlideSpeedMax) // limit wallsliding speed
            {
                velocity.y = -wallSlideSpeedMax;
            }
            if (timeToWallUnstick > 0)
            {
                velocity.x = 0;
                velocityXSmoothing = 0;

                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing,
            (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }

}