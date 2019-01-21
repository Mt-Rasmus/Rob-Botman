using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {

    public enum SpawnState
    {
        SPAWNING, 
        WAITING, 
        COUNTING
    };

    [System.Serializable]
    public class Wave
    {
        [HideInInspector]
        public string waveName;
        public GameObject enemy;
        public int enemyAmount;
        public float spawnRate;
    }

    public Wave wave;
    private int nextWave = 1;
    public float timeBetweenWaves = 5f;
    private float waveCountdown;
    public string theWaveName;
    private float buffChance = 0.5f;

    public Transform[] spawnPoints;

    private SpawnState state = SpawnState.COUNTING;

    public float searchCountdown = 1f;

	void Start () {
        if(spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points referenced");
        }
        waveCountdown = timeBetweenWaves;
	}
	
	void Update () {

        if(state == SpawnState.WAITING)
        {
            if (!EnemyIsAlive())
            {
                WaveCompleted(wave);
            }
            else
            {
                return;
            }
        }
		if(waveCountdown <= 0)
        {
            if(state != SpawnState.SPAWNING)
            {
                // spawn wave
                StartCoroutine(SpawnWave(wave));  
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }
    }

    void WaveCompleted(Wave _wave)
    {
        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        _wave.enemyAmount++;
        nextWave++;
        _wave.waveName = "Wave " + nextWave;

        
    }

    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;

        if(searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                // Begin a new round
                return false;
            }
        }
        
        return true;
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        state = SpawnState.SPAWNING;
        theWaveName = "Wave " + nextWave;
        // spawn
        for (int i = 0; i < _wave.enemyAmount; i++)
        {
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds(1/_wave.spawnRate);
        }

        state = SpawnState.WAITING;

        yield break;
    }

    private void SpawnEnemy(GameObject _enemy)
    {
        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyInstance = Instantiate(_enemy, _sp.position, _sp.rotation);
        //if(Random.Range(0f,1f) <= buffChance)
        //{
        //    Debug.Log("BUFFED ENEMY SPAWNED");
            
        //}
    }
}
