using UnityEngine;
using System.Collections;

public class Column : MonoBehaviour 
{
	void OnTriggerEnter2D(Collider2D other)
	{
		Bird bird = other.GetComponent<Bird>();
		if(bird != null)
		{
			// Bird hit the column:
		}
	}
}
