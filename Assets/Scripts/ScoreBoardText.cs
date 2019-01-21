using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoreBoardText : MonoBehaviour {

    public Text mytext = null;
    private GameObject thePlayer;
    private Player PlayerComp;
    private bool textPresent;

    // Use this for initialization
    void Start()
    {
        thePlayer = GameObject.FindGameObjectWithTag("Player");
        PlayerComp = thePlayer.GetComponent<Player>();
        mytext.text = "Score " + PlayerComp.score;
    }

    // Update is called once per frame
    void Update()
    {
        mytext.text = "Score: " + PlayerComp.score;
    }

}
