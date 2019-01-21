using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Player controller taken in large part from this tutorial:
 * https://github.com/SebLague/2DPlatformer-Tutorial
 * https://www.youtube.com/playlist?list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz
 */

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour
{

    Player player;
    [HideInInspector]
    public Vector2 directionalInput;
    public GameObject menu;

    // Use this for initialization
    void Start () {
        player = GetComponent<Player>();
    }
	
	// Update is called once per frame
	void Update () {

        directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);

        if (Input.GetKeyDown(KeyCode.Space) && !menu.activeSelf)
        {
            player.OnJumpInputDown();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.SetActive(menu.activeSelf ? false : true);
            if (menu.activeSelf)
            {
                Time.timeScale = 0;
            }
            if (!menu.activeSelf)
            {
                Time.timeScale = 1;
            }
        }   
    }
}
