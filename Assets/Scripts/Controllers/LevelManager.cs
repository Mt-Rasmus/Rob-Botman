using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public float respawnDelay;
    public Player gamePlayer;

    // Use this for initialization
    void Start()
    {
        gamePlayer = FindObjectOfType<Player>();
    }

    public void Respawn()
    {
        StartCoroutine("RespawnCoroutine");
    }

    public IEnumerator RespawnCoroutine()
    {
        gamePlayer.gameObject.SetActive(false); // despawn 
        yield return new WaitForSeconds(respawnDelay); // delay
        gamePlayer.transform.position = gamePlayer.respawnPoint;
        gamePlayer.gameObject.SetActive(true); // respawn 
    }
}
