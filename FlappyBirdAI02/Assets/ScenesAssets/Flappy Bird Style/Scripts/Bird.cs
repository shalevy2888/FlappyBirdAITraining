using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour 
{
	private Animator anim;					//Reference to the Animator component.
	private Rigidbody2D rb2d;				//Holds a reference to the Rigidbody2D component of the bird.
	private bool isDead = false;
	public float upForce = 160;
	void Awake() {
		//Get reference to the Animator component attached to this GameObject.
		anim = GetComponent<Animator> ();
		//Get and store a reference to the Rigidbody2D attached to this GameObject.
		rb2d = GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate()
	{
		if (isDead) return;
		if (Input.GetMouseButtonDown(0)) {
			//...tell the animator about it and then...
			anim.SetTrigger("Flap");
			//...zero out the birds current y velocity before...
			rb2d.velocity = Vector2.zero;
			rb2d.AddForce(new Vector2(0,upForce));
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		// Zero out the bird's velocity:
		rb2d.velocity = Vector2.zero;

		//...tell the Animator about it...
		anim.SetTrigger ("Die");
		isDead = true;
	}
}
