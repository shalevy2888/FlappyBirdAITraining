using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour 
{
	private Animator anim;					//Reference to the Animator component.
	private Rigidbody2D rb2d;				//Holds a reference to the Rigidbody2D component of the bird.
	private bool isDead = false;
	public float upForce = 160;
	public int score = 0;
	private Vector2 initialPosition;
	void Awake() {
		//Get reference to the Animator component attached to this GameObject.
		anim = GetComponent<Animator> ();
		//Get and store a reference to the Rigidbody2D attached to this GameObject.
		rb2d = GetComponent<Rigidbody2D>();
		initialPosition = transform.position;
	}

	public void Restart() {
		transform.position = initialPosition;
		isDead = false;
		score = 0;
		rb2d.velocity = Vector2.zero;
		transform.rotation = Quaternion.identity;
		rb2d.angularVelocity = 0;
	}
	
	void FixedUpdate()
	{
		if (isDead) return;
		if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) {
			//...tell the animator about it and then...
			anim.SetTrigger("Flap");
			//...zero out the birds current y velocity before...
			rb2d.velocity = Vector2.zero;
			rb2d.AddForce(new Vector2(0,upForce));
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		// Hitting the column or ground

		// Zero out the bird's velocity:
		rb2d.velocity = Vector2.zero;

		//...tell the Animator about it...
		anim.SetTrigger ("Die");
		isDead = true;
		GameLogic.GetInstance().BirdDied();
	}
}
