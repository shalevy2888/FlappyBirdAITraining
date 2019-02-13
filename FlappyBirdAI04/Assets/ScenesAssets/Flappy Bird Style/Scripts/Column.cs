using UnityEngine;
using System.Collections;

public class Column : MonoBehaviour 
{
	public float xPosToDestroy;
	void OnTriggerEnter2D(Collider2D other)
	{
		Bird bird = other.GetComponent<Bird>();
		if(bird != null)
		{
			// Bird moved between the two column triggered the collider that is in between the columns, but just after them:
			GameLogic.GetInstance().BirdScored(bird);
		}
	}
	void Update()
	{
		if (transform.position.x < xPosToDestroy) {
			Destroy(gameObject);
		}
	}
}
