using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {


    [SerializeField] Behaviour[] componentsToDisable;

    //Player Player;

    private void Start()
    {
        //Player = GetComponent<Player>();

        if (!isLocalPlayer)
        {
            //Ball2 ballScript = GetComponentInChildren<Ball2>();
            //ballScript.enabled = false;

            for (int i = 0; i< componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }
    }

}
