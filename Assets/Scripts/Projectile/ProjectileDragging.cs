using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDragging : MonoBehaviour {

	public float maxStretch = 3.0f;
	public LineRenderer slingshotLineFront;
	public LineRenderer slingshotLineBack;

	private Transform Slingshot;
	private SpringJoint2D spring;
	private Ray rayToMouse;
	private Ray leftSlingshotToProjectile; // ray from projectile to front arm 
	private float maxStretchSqr;
	private float projectileRadius;
	private bool clickedOn;
	private Vector2 prevVelocity;
	private Rigidbody2D rb;

	void Awake () {
		spring = GetComponent<SpringJoint2D> ();
		Slingshot = spring.connectedBody.transform;
	}

	// Use this for initialization
	void Start () {
		slingshotLineFront.SetPosition(0, slingshotLineFront.transform.position);
		slingshotLineBack.SetPosition(0, slingshotLineBack.transform.position);

		rayToMouse = new Ray(Slingshot.position, Vector3.zero);	
		leftSlingshotToProjectile = new Ray(slingshotLineFront.transform.position, Vector3.zero);

		maxStretchSqr = maxStretch * maxStretch;

//		CircleCollider2D circle = collider2D as CircleCollider2D;
		CircleCollider2D circle = GetComponent<CircleCollider2D> ();
		rb = GetComponent<Rigidbody2D> ();

		projectileRadius = circle.radius;
	}
	
	// Update is called once per frame
	void Update () {
		if (clickedOn)
			Dragging ();

		if (spring != null) { // projectile not launched yet

			if (rb.bodyType != RigidbodyType2D.Kinematic && prevVelocity.sqrMagnitude > rb.velocity.sqrMagnitude) {
				Destroy (spring);
				rb.velocity = prevVelocity; // restore velocity so that projectile doesn't just fall down imediately.
			}

			if (!clickedOn)
				prevVelocity = rb.velocity;

			LineRendererUpdate ();

		} else { // projectile launched
			slingshotLineFront.enabled = false;
			slingshotLineBack.enabled = false;
		}
	}

	void OnMouseDown () {
		spring.enabled = false;
		clickedOn = true;
	}

	void OnMouseUp () {
		spring.enabled = true;
		// rb.isKinematic = false;
		//  Rigidbody2D.bodyType
		rb.bodyType = RigidbodyType2D.Dynamic;
		clickedOn = false;
	}

	void Dragging () {
		Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		Vector2 SlingshotToMouse = mouseWorldPoint - Slingshot.position; // Vector between projectile and slingshot

		if (SlingshotToMouse.sqrMagnitude > maxStretchSqr) {
			rayToMouse.direction = SlingshotToMouse;
			mouseWorldPoint = rayToMouse.GetPoint(maxStretch); // if over maxstretch, set projectile pos to maxStretch point
					
		}

		mouseWorldPoint.z = 0f;
		transform.position = mouseWorldPoint;
	}

	void LineRendererUpdate () {
		Vector2 slingshotToProjectile = transform.position - slingshotLineFront.transform.position;
		leftSlingshotToProjectile.direction = slingshotToProjectile;
		Vector3 holdPoint = leftSlingshotToProjectile.GetPoint(slingshotToProjectile.magnitude + projectileRadius); // point at the back end of the projectiles

		slingshotLineFront.SetPosition(1, holdPoint); // anchor at the back of the projectile!
		slingshotLineBack.SetPosition(1, holdPoint); // anchor at the back of the projectile!
	}
}
