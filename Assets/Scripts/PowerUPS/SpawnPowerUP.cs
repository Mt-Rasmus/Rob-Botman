using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// helper struct to keep track of which PU:s are present where
struct PU_tracker
{
    public int spawnPt;
    public GameObject PU;
    public PU_tracker(int pt, GameObject pu)
    {
        spawnPt = pt;
        PU = pu;
    }
}

struct PB_tracker
{
    public int spawnPt;
    public GameObject PB;
    public Transform loadingBar;
    public PB_tracker(int pt, GameObject pb, Transform lb)
    {
        spawnPt = pt;
        PB = pb;
        loadingBar = lb;
    }
}

public class SpawnPowerUP : MonoBehaviour {

    [SerializeField] private Transform[] spawnPoints; // set spawn points in editor
    [SerializeField] private GameObject[] powerUPS; // set powerup prefabs in editor
    [SerializeField] private GameObject[] PrograssBars; // set progressbar prefabs in editor
    [SerializeField] private Transform[] PrograssBarSpawns; // set progressbar prefabs in editor

    public AudioClip HealthUpSound;
    public AudioClip DDSound;
    public AudioClip SpeedBoostSound;
    public AudioClip FastProjSound;
    public AudioSource audiosource1;
    public float PowerUpDuration;

    private GameObject HealthBar;
    private Hashtable PUhash;
    private Hashtable PBhash;
    private GameObject[] PU_objects;
    private int[] PU_keys;
    private List<PU_tracker> PU_list;
    private List<PB_tracker> PB_list;
    private GameObject spawnedPU;
    private GameObject thePlayer;
    private Player PlayerComp;
    private GameObject[] PB_objects;
    private Transform LoadingBar;
    private float startTime;
    private float baseSpeedPlayer;
    private float baseDamagePlayer;
    private float baseWallSlideSpeedMax;
    private float baseWallJumpClimbY;
    private int nrOfSpeedPUs = 0;
    private int nrOfDDs = 0;
    private int nrOfFPs = 0;

    // Use this for initialization
    void Start () {

        PUhash = new Hashtable(); //  stores power ups
        PBhash = new Hashtable(); //  stores progress bar objects
        PU_list = new List<PU_tracker>();
        PB_list = new List<PB_tracker>();
        StartCoroutine(timedSpawn());
        startTime = Time.time;
        PowerUpDuration = 10f;
        baseSpeedPlayer = 10f;
        baseDamagePlayer = 50f;
        baseWallSlideSpeedMax = 3f;
        baseWallJumpClimbY = 16f;

}
	
	void Update () {

        // Section to decrease power up bar time and remove if expired
        if (PB_list.Count > 0)
        {
            foreach (PB_tracker obj in PB_list.ToList())
            {
                obj.loadingBar.GetComponent<Image>().fillAmount -= (1.0f / PowerUpDuration) * Time.deltaTime;

                if (obj.loadingBar.GetComponent<Image>().fillAmount < 0.00001f)
                {
                    if (obj.PB.name == "PuProgressBar1(Clone)")
                    {
                        if(nrOfSpeedPUs > 0)
                            nrOfSpeedPUs--;
                        if (nrOfSpeedPUs == 0)
                        {
                            PlayerComp.moveSpeed = baseSpeedPlayer;
                            PlayerComp.wallSlideSpeedMax = baseWallSlideSpeedMax;
                            PlayerComp.wallJumpClimb.y = baseWallJumpClimbY;
                        }
                            
                    }
                    else if (obj.PB.name == "PuProgressBar2(Clone)")
                    {
                        if (nrOfDDs > 0)
                            nrOfDDs--;
                        if (nrOfDDs == 0)
                            PlayerComp.damage = baseDamagePlayer;
                    }
                    else if (obj.PB.name == "PuProgressBar3(Clone)")
                    {
                        if (nrOfFPs > 0)
                            nrOfFPs--;
                        if (nrOfFPs == 0)
                            PlayerComp.BallPowerUP = false;
                    }                     

                    PB_list.Remove(obj);
                //    PUhash.Remove(obj.spawnPt);
                    Destroy(obj.PB);

                }

            }
        }

        // Section to remove power ups that are touched by the player.
        if (PU_list.Count > 0)
        {
            foreach (PU_tracker obj in PU_list.ToList())
            {
                if (obj.PU && obj.PU.GetComponent<Collider2D>().IsTouchingLayers(LayerMask.GetMask("Player")))
                {
                    PowerUPspawn(obj.PU);
                    PU_list.Remove(obj);
                    PUhash.Remove(obj.spawnPt);
                    Destroy(obj.PU);
                }
            }
        }

    }

    private void PowerUPspawn(GameObject powerUp)
    {
        thePlayer = GameObject.FindGameObjectWithTag("Player");
        PlayerComp = thePlayer.GetComponent<Player>();

        Ball2 BallComp = thePlayer.GetComponent<Ball2>();
        int PBindex = 0;

        // Apply power up effect

        if (powerUp.name == "PU Extra Speed(Clone)")
        {
            PBindex = 0;
            audiosource1.PlayOneShot(SpeedBoostSound);

            if (PlayerComp.moveSpeed == baseSpeedPlayer)
            {
                PlayerComp.moveSpeed += 10f;
                PlayerComp.wallSlideSpeedMax += 5f;
                PlayerComp.wallJumpClimb.y += 10f;
            }
        }
        else if (powerUp.name == "PU Double Damage(Clone)")
        {
            audiosource1.PlayOneShot(DDSound);
            PlayerComp.damage *= 2;
            PBindex = 1;
        }
        else if (powerUp.name == "PU Faster Projectile(Clone)")
        {
            audiosource1.PlayOneShot(FastProjSound);
            PBindex = 2;
            PlayerComp.BallPowerUP = true;

        //    if (!PBhash.Contains(2))
        //        BallComp.damping -= 0.3f;

        }
        else if (powerUp.name == "PU HealthUP(Clone)")
        {
            audiosource1.PlayOneShot(HealthUpSound);
            float hpUP = 25f;

            if (PlayerComp.HP < PlayerComp.baseHP)
            {
                if(PlayerComp.HP + hpUP > PlayerComp.baseHP)
                    PlayerComp.HP = PlayerComp.baseHP;
                else
                    PlayerComp.HP += hpUP;

                IncreaseHealthBar(hpUP);
            }
                
        }

        string powerUpnName = powerUp.name;

        if(powerUp.name != "PU HealthUP(Clone)")
            SpawnPowerUPBar(powerUp, PBindex);

    }

    public void IncreaseHealthBar(float hp)
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
                                    l.GetComponent<Image>().fillAmount += hp / 100;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void SpawnPowerUPBar(GameObject powerUp, int PBindex)
    {

        Vector2 progPos = new Vector2(0f, 0f);
        Vector2 posBar = thePlayer.transform.position;
        GameObject hb = GameObject.FindGameObjectWithTag("HealthBar");

        if (PBindex == 0)
            progPos = new Vector3(posBar.x + 0.5f - 2f, posBar.y + 11.85f / 1.5f, transform.position.z);
        else if(PBindex == 1)
            progPos = new Vector3(posBar.x + 0.5f, posBar.y + 11.85f / 1.5f, transform.position.z);
        else
            progPos = new Vector3(posBar.x + 0.5f + 2f, posBar.y + 11.85f / 1.5f, transform.position.z);


        GameObject ProgressBar = Instantiate(PrograssBars[PBindex], progPos, transform.rotation);
        //      GameObject ProgressBar = Instantiate(PrograssBars[PBindex], PrograssBarSpawns[PBindex].position, transform.rotation);

        // Extract

        foreach (Transform t in ProgressBar.transform)
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
                                Transform yo = l;

                                if (l != null)
                                {
                                    startTime = Time.time;
                                    LoadingBar = l;

                                    if (!PBhash.Contains(PBindex))
                                        PBhash.Add(PBindex, l);
                                    else
                                        l.GetComponent<Image>().fillAmount = 1.0f;

                                    if(ProgressBar.name == "PuProgressBar1(Clone)") 
                                        nrOfSpeedPUs = nrOfSpeedPUs + 1;
                                    else if (ProgressBar.name == "PuProgressBar2(Clone)")
                                        nrOfDDs = nrOfDDs + 1;
                                    else if (ProgressBar.name == "PuProgressBar3(Clone)")
                                        nrOfFPs = nrOfFPs  + 1;

                                        PB_list.Add(new PB_tracker(PBindex, ProgressBar, LoadingBar));
                                    StartCoroutine(DestroyProgressBar(ProgressBar));
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator DestroyProgressBar(GameObject powerUp)
    {
        yield return new WaitForSeconds(2f); // must have IEnumerator for this (coroutine)
    //    Destroy(powerUp);
    }

    IEnumerator timedSpawn()
    {
        while (true)
        {
            float timeInterval;

            // Powerup spawn interval (quicker the longer the game goes on)

            if (Time.time - startTime > 60)
                timeInterval = Random.Range(3f, 5f);
            else
                timeInterval = Random.Range(5f, 10f);

            bool foundSlot = false;
            int SpawnPtIdx = Random.Range(0, spawnPoints.Length); // Picks a random index for Power up spawn point

            // Section to make sure that spawn point index is changed to one that is not "taken", if a slot is open.
            if (PUhash != null && PUhash.Count < spawnPoints.Length)
            {
                while (!foundSlot)
                {
                    if (PUhash.ContainsKey(SpawnPtIdx))
                        SpawnPtIdx = Random.Range(0, spawnPoints.Length);
                    else
                        foundSlot = true;
                }
            }

            yield return new WaitForSeconds(timeInterval);

            int PUidx = Random.Range(0, powerUPS.Length); // Picks a random index for Power up prefab

            // Spawns new powerup if the spawn point is not "taken" by another presently
            if (PUhash != null && PU_list != null && !PUhash.ContainsKey(SpawnPtIdx))
            {
                spawnedPU = Instantiate(powerUPS[PUidx], spawnPoints[SpawnPtIdx].position, transform.rotation);
                PUhash.Add(SpawnPtIdx, spawnedPU);
                PU_list.Add(new PU_tracker(SpawnPtIdx, spawnedPU));
            }  

        }
    }

}