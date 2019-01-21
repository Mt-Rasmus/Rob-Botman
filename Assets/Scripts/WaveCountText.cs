using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class WaveCountText : MonoBehaviour
{

    public Text mytext = null;
    private GameObject enemySpawn;
    private EnemySpawn enemySpawnComp;
    private bool textPresent = false;
    private int wave;

    // Use this for initialization
    void Start()
    {
        enemySpawn = GameObject.FindGameObjectWithTag("EnemySpawn");
        enemySpawnComp = enemySpawn.GetComponent<EnemySpawn>();
        mytext.text = enemySpawnComp.theWaveName;
        wave = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemySpawnComp.theWaveName != null)
        {
            StartCoroutine(WaveText());
        }

    }

    IEnumerator WaveText()
    {

        mytext.text = enemySpawnComp.theWaveName;

        int duration = 4;

        if (wave == 1)
            duration = 7;

        yield return new WaitForSeconds(duration);
        mytext.text = "";
        wave++;
        enemySpawnComp.theWaveName = null;
    }
}
