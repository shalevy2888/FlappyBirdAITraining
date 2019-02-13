using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour 
{
	private Animator anim;					//Reference to the Animator component.
	private Rigidbody2D rb2d;				//Holds a reference to the Rigidbody2D component of the bird.

	void Awake() {
		//Get reference to the Animator component attached to this GameObject.
		anim = GetComponent<Animator> ();
		//Get and store a reference to the Rigidbody2D attached to this GameObject.
		rb2d = GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate()
	{
		
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		// Zero out the bird's velocity:
		rb2d.velocity = Vector2.zero;

		//...tell the Animator about it...
		anim.SetTrigger ("Die");
	}
}
