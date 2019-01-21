using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AimingRotation : MonoBehaviour {

    private float angle;
    private bool facingLeft = false;
    private bool isPressed = false;
    private SpriteRenderer playerRender;
    private Player player;

    private Vector2 mousePos0; // first click
    private Vector2 mousePos0Follow; // first click + player movement
    private Vector2 mousePos;

    public GameObject menu;

    void Start () {
        
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerRender = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>();
        mousePos0.Set(0f, 0f);
        mousePos.Set(0f, 0f);
    }

    void Update () {

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetButtonDown("Fire1"))
        {
            mousePos0 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isPressed = true;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            isPressed = false;
        }
        if (isPressed)
        {
            // update mousePos0Follow to follow the player movements
            //            mousePos0Follow.x = mousePos0.x + player.transform.position.x;
            //            mousePos0Follow.y = mousePos0.y + player.transform.position.y;
            mousePos0Follow.x = mousePos0.x;
            mousePos0Follow.y = mousePos0.y;

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (mousePos.x < mousePos0Follow.x && facingLeft || mousePos.x > mousePos0Follow.x && !facingLeft && !menu.activeSelf)
            {   
                // Rotate player to match aim direction
                RotatePlayer();
            }
        }
        // Rotate player to match key input direction, if not currently aiming
        else if (!isPressed && (facingLeft && Input.GetKeyDown("d") || !facingLeft && Input.GetKeyDown("a")) && !menu.activeSelf)
        {
            RotatePlayer();
        }
    }

    private void RotatePlayer()
    {
        transform.Rotate(new Vector3(1, 0, 0), 180); // rotate around x-axis
        playerRender.flipX = facingLeft ? false : true;
        facingLeft = facingLeft ? false : true; 

    }

}
