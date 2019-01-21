using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    private Vector2 velocity;
    private float smoothTimeX, smoothTimeY;
    private float startX, startY;
    private float posX, posY;
    public GameObject player, hb;
    public GameObject[] ProgBars;
    //private Transform cam;

    public Vector3 minCameraPos;
    public Vector3 maxCameraPos;

	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        hb = GameObject.FindGameObjectWithTag("HealthBar");
        //cam = Camera.main.transform;
        
        startX = hb.transform.position.x;
        startY = hb.transform.position.y;
    }
	
	void Update () {

        smoothTimeX = 0.05f;
        smoothTimeY = 0.05f;
        player = GameObject.FindGameObjectWithTag("Player");
        ProgBars = GameObject.FindGameObjectsWithTag("PU_Progressbar");

        if (player)
        {
            posX = Mathf.SmoothDamp(transform.position.x, player.transform.position.x, ref velocity.x, smoothTimeX);
            posY = Mathf.SmoothDamp(transform.position.y, player.transform.position.y, ref velocity.y, smoothTimeX);

            transform.position = new Vector3(Mathf.Clamp(posX, minCameraPos.x, maxCameraPos.x),
                Mathf.Clamp(posY, minCameraPos.y, maxCameraPos.y), Mathf.Clamp(transform.position.z, minCameraPos.z, maxCameraPos.z));

            hb.transform.position = new Vector3(transform.position.x, transform.position.y + startY / 1.3f, transform.position.z);
        }

        if(ProgBars != null && ProgBars.Length > 0)
        {
            for(int i = 0; i < ProgBars.Length; i++)
            {  
                if (ProgBars[i].name == "PuProgressBar1(Clone)")
                {
                    ProgBars[i].transform.position = new Vector3(transform.position.x - 2.5f, hb.transform.position.y - 1.5f, transform.position.z);
                }
                else if(ProgBars[i].name == "PuProgressBar2(Clone)")
                {
                    ProgBars[i].transform.position = new Vector3(transform.position.x - 0.5f, hb.transform.position.y - 1.5f, transform.position.z);
                }
                else if(ProgBars[i].name == "PuProgressBar3(Clone)")
                {
                    ProgBars[i].transform.position = new Vector3(transform.position.x + 1.5f, hb.transform.position.y - 1.5f, transform.position.z);
                }
            }
        }
    }
}
