using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject player;
    public float offset;
    public float offsetSmoothing;
    private Vector3 playerPosition;
    private SpriteRenderer playerRender;


    // Use this for initialization
    void Start()
    {
        playerRender = player.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = new Vector3(player.transform.position.x,
            transform.position.y, transform.position.z);

        if (!playerRender.flipX)
        {
            playerPosition = new Vector3(playerPosition.x + offset,
                playerPosition.y, playerPosition.z);
        }
        else
        {
            playerPosition = new Vector3(playerPosition.x - offset,
                playerPosition.y, playerPosition.z);
        }
        transform.position = Vector3.Lerp(transform.position, playerPosition, offsetSmoothing * Time.deltaTime);
    }
}
