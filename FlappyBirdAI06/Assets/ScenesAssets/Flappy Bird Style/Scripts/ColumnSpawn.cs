using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnSpawn : MonoBehaviour {
	public GameObject columnPrefab;									//The column game object.
	public float spawnRate = 3f;									//How quickly columns spawn.
	public float columnMin = -1f;									//Minimum y value of the column position.
	public float columnMax = 3.5f;									//Maximum y value of the column position.

	private float spawnXPosition = 10f;

	private float timeSinceLastSpawned;
	public float xPosColumnNotVisible;
	

	// Use this for initialization
	void Start () {
		timeSinceLastSpawned = spawnRate;
	}
	
	// Update is called once per frame
	void Update () {
		timeSinceLastSpawned += Time.deltaTime;

		if (timeSinceLastSpawned >= spawnRate) 
		{	
			timeSinceLastSpawned = 0f;

			//Set a random y position for the column
			float spawnYPosition = Random.Range(columnMin, columnMax);
			// Spawn the object and set position.
			Column column = Instantiate(columnPrefab, new Vector2(spawnXPosition, spawnYPosition) , Quaternion.identity).GetComponent<Column>();
			column.xPosToDestroy = xPosColumnNotVisible;

			// Assign the new column parent to the ColumnSpawn object transform, such that we
			// can iterate over it's children later on, see Restart() and GetNearestColumn() 
			column.transform.SetParent(transform);
		}
	}

	public void Restart() {
		timeSinceLastSpawned = spawnRate;
		List<Transform> childList = new List<Transform>();
		for (int i = 0; i < transform.childCount; i++) {
			childList.Add(transform.GetChild(i));
		}
		foreach (var trans in childList)
		{
			Destroy(trans.gameObject);
		}
	}
	// *** AI
	float deltaX = 0.80f;
	public int GetNearestColumn(Transform from, out float horizontalDistance, out float heightDifference) {
		int bestFound = -1;
		horizontalDistance = 30;
		heightDifference = from.position.y;

		for (int i = 0; i < transform.childCount; i++) {
			float testDistance = (transform.GetChild(i).transform.position.x+deltaX) - from.position.x;
			if (testDistance >0 && testDistance<horizontalDistance) {
				bestFound = i;
				horizontalDistance = testDistance;
				heightDifference = from.position.y - transform.GetChild(i).transform.position.y;
			}
		}
		return bestFound;
	}
}
