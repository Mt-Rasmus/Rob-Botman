using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Ball : MonoBehaviour {

	public Rigidbody2D rb;
    public Rigidbody2D rbCopy;
    public Rigidbody2D hook;
	public float releaseTime = .15f;
	public float maxDragDistance = 2f;
	public GameObject nextBall;
	public GameObject newBall;
	SpringJoint2D Spring_newBall;
	//GameObject newBall2;
    //List<GameObject> objects;

    private Transform Slingshot;
    [HideInInspector]
	public bool isPressed = false;
	private SpringJoint2D spring;
    [HideInInspector]
    public Vector2 mousePos;
    [HideInInspector]
    public Vector2 mousePos0;
	Vector2 newRbPos;
	Vector2 rbPos0;
	private bool firstClick;
    private bool firstBall;
    Rigidbody2D newBall2;
    private int ballCount;

    void Start () {

        firstBall = true;
        ballCount = 0;
        spring = GetComponent<SpringJoint2D> ();
		Slingshot = spring.connectedBody.transform;
		newRbPos = new Vector2(0.0f, 0.0f);
		firstClick = false;

		rb.freezeRotation = true;
		spring.dampingRatio = 1f;
		spring.frequency = 0f;

       // List<GameObject> objects = new List<GameObject>();
    }

	void Update () {

        if(firstBall) { 

		    if (isPressed) {
                
                // Save startposition where first clicked down
                if (isPressed && firstClick == false) {

				    rbPos0 = rb.position;
				    mousePos0 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				    firstClick = true;
			    }

			    mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			    newRbPos = rbPos0 + (mousePos - mousePos0);

			    if (Vector3.Distance(newRbPos, hook.position) > maxDragDistance) {
				    rb.position = hook.position + (newRbPos - hook.position).normalized * maxDragDistance;
			    }
			    else
				    rb.position = newRbPos;				
		    }

		    if (Input.GetButtonDown("Fire1")) {

			    rb.freezeRotation = false;
			    spring.dampingRatio = 0f;
			    spring.frequency = 1.5f;
			    isPressed = true;
			    rb.isKinematic = true;			
		    }

		    if(Input.GetButtonUp("Fire1")) {
			    isPressed = false;
			    rb.isKinematic = false;
                firstBall = false;
			    StartCoroutine(Release());			
		    }
        }
        else
        {
            if (isPressed)
            {
                // Save startposition where first clicked down
                if (isPressed && firstClick == false)
                {

                    rbPos0 = newBall2.position;
                    mousePos0 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    firstClick = true;
                }

                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                newRbPos = rbPos0 + (mousePos - mousePos0);

                if (Vector3.Distance(newRbPos, hook.position) > maxDragDistance)
                {
                    newBall2.position = hook.position + (newRbPos - hook.position).normalized * maxDragDistance;
                }
                else
                    newBall2.position = newRbPos;
            }

            if (Input.GetButtonDown("Fire1"))
            {

                newBall2.freezeRotation = false;
                spring.dampingRatio = 0f;
                spring.frequency = 1.5f;
                isPressed = true;
                newBall2.isKinematic = true;
            }

            if (Input.GetButtonUp("Fire1"))
            {
                isPressed = false;
                newBall2.isKinematic = false;
                
                StartCoroutine(Release());
            }
        }
    }

    IEnumerator Release()
    {
        yield return new WaitForSeconds(releaseTime); // must have IEnumerator for this (coroutine)
        GetComponent<SpringJoint2D>().enabled = false;
        StartCoroutine(spawnBall());

    }

    IEnumerator spawnBall()
    {
        yield return new WaitForSeconds(0.5f); // must have IEnumerator for this (coroutine)
        
  //      if(rbCopy) {
            newBall2 = Instantiate(rbCopy, Slingshot.position, transform.rotation);

        ballCount++;

        newBall2.name = "Ball" + ballCount.ToString();
/*
        Destroy(newBall2, 2f);

        ballCount++;

        newBall2.name = "Ball" + ballCount.ToString();

        GameObject go = GameObject.Find("Ball");

        if (go)
            Destroy(go, 2f);

        var objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == newBall2.name || obj.name == "Ball2");

        int next = 0;

        print("Number of balls in objects: " + objects.Count<GameObject>());

        foreach (GameObject go0 in objects)
        {
            next = next + 1;
            go0.name = "Ball" + next.ToString();
            print("go0.name = " + go0.name);
            if (go0 && go0.name != "Ball1" && objects.Count<GameObject>() > 2) {
            //    go0.gameObject.SetActive(false);
                Destroy(go0, 0.1f);
            }
        }

        print("Number of balls in objectsssssssssssssssssss: " + objects.Count<GameObject>());
*/

        newBall2.GetComponent<Ball>().hook = hook;
        newBall2.GetComponent<Ball>().newBall = newBall;
        newBall2.GetComponent<SpringJoint2D>().connectedBody = hook;
        newBall2.GetComponent<SpringJoint2D>().enabled = true;

        //rb = newBall2.GetComponent<Rigidbody2D>();
        newBall2.freezeRotation = true;
        newBall2.gameObject.SetActive(true);
//        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //Check collision name
        Debug.Log("collision name = " + col.collider.gameObject.name);
          if (col.gameObject.name == "Ball(Clone)" || col.gameObject.name == "Ball")
          {
            col.gameObject.SetActive(false);
            Destroy(col.gameObject);
          }
    }
    
}
