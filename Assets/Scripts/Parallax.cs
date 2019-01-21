using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {

    public Transform background;
    private float parallaxScale;
    public float smoothing = 1f;
    private float parallax;
    private float backgroundTargetPosX;
    private Vector3 backgroundTargetPos;

    private Transform cam;
    private Vector3 previousCamPos;

    private void Awake()
    {
        cam = Camera.main.transform;
    }

    // Use this for initialization
    void Start () {
        previousCamPos = cam.position;
        parallaxScale = background.position.z * -1f;
        parallax = 0f;
        backgroundTargetPosX = 0f;
        backgroundTargetPos = new Vector3();
    }
	
	// Update is called once per frame
	void Update () {

        parallax = (previousCamPos.x - cam.position.x) * parallaxScale;

        backgroundTargetPosX = background.position.x + parallax;

        backgroundTargetPos = new Vector3(backgroundTargetPosX, background.position.y, background.position.z);

        background.position = Vector3.Lerp(background.position, backgroundTargetPos, smoothing * Time.deltaTime);

        previousCamPos = cam.position;
	}
}
