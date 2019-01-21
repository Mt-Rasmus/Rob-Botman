using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // movement
    public float speed = 5f;
    public float jumpSpeed = 5f;
    private float movement = 0f;
    private Rigidbody2D rigidBody;
    private Collider2D playerCollider;
    private SpriteRenderer playerRender;
    // jump checks
    public float maxJumps = 1f;
    private float jumpCounter = 0f;
    public float jumpForce = 1000f;
    public float jumpPushForce = 1000f;
    public Transform groundCheckPoint;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    private bool isTouchingGround;
    private bool isTouchingWall;
    // animation
    //private Animator playerAnimation;
    // respawn
    public Vector3 respawnPoint;
    public LevelManager gameLevelManager;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        //playerAnimation = GetComponent<Animator>();
        playerRender = GetComponent<SpriteRenderer>();
        respawnPoint = transform.position;
        gameLevelManager = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        isTouchingGround = Physics2D.IsTouchingLayers(playerCollider, groundLayer);
        isTouchingWall = Physics2D.IsTouchingLayers(playerCollider, wallLayer);

        movement = Input.GetAxis("Horizontal"); // (-1,1), left/right

        if (movement > 0.01f)
        {
            rigidBody.velocity = new Vector2(movement * speed, rigidBody.velocity.y);
            playerRender.flipX = false;
        }
        else if (movement < 0.01f)
        {
            rigidBody.velocity = new Vector2(movement * speed, rigidBody.velocity.y);
            playerRender.flipX = true;
        }
        else
        {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        }

        if (Input.GetButtonDown("Jump") && jumpCounter < maxJumps)
        {
            jumpCounter++;
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpSpeed);
        }
        if(Input.GetButtonDown("Jump") && isTouchingWall)
        {
            WallJump();
            //rigidBody.velocity = new Vector2(jumpSpeed, jumpSpeed);
        }
        if (isTouchingGround)
        {
            jumpCounter = 0;
        }

        //playerAnimation.SetFloat("Speed", Mathf.Abs(rigidBody.velocity.x));
        //playerAnimation.SetBool("onGround", isTouchingGround);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "FallDetector")
        {
            Debug.Log("Collide");
            gameLevelManager.Respawn();
        }
        if (collision.tag == "Checkpoint")
        {
            respawnPoint = collision.transform.position;
        }
    }

    void WallJump()
    {
        rigidBody.AddForce(new Vector2(jumpForce, 10f));// = new Vector2(jumpSpeed, jumpSpeed);
    }
}
