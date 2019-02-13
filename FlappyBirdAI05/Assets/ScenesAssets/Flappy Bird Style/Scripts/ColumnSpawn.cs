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
}
