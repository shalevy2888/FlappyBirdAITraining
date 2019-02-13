using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour {
	private static GameLogic instance = null;
	public static GameLogic GetInstance() { return instance; }
	private bool gameOver = false;
	private bool gamePaused;
	public bool GameOver {
		get { return gameOver; }
	}
	public Text scoreText;
	public Text centerText;
	List<Bird> birds = new List<Bird>();
	public GameObject birdPrefab;
	public int numberOfBirds;
	int maxScore;
	int aliveBirds;

	private const string WellcomeText = "Flappy Bird, press Return to start.";
	private const string GameOverText = "Game Over, press Return to start.";
	// Use this for initialization
	void InstantiateBirds() {
		for (int i = 0; i < numberOfBirds; i++) {
			birds.Add(Instantiate(birdPrefab, Vector2.zero, Quaternion.identity).GetComponent<Bird>());
		}
	}
	
	void Awake() {
		instance = this;
	}
	void Start () {
		centerText.text = WellcomeText;
		Time.timeScale = 0;
		gamePaused = true;
		InstantiateBirds();

		Restart();
	}

	void Restart() {
		maxScore = 0;
		aliveBirds = numberOfBirds;
		// Restart game:
		// 1. restart Bird:
		foreach (var bird in birds)
		{
			bird.Restart();
		}
		GetComponent<ColumnSpawn>().Restart();
	}
	// Update is called once per frame
	void Update () {
		if (gamePaused == true) {
			if (Input.GetKeyDown(KeyCode.Return)) {
				// Un pause the game either start (first time playing), or restart
				if (gameOver == true) {
					Restart();
				}
				centerText.text = "";
				Time.timeScale = 1;
				gamePaused = false;
			}
		}
	}

	public void BirdScored(Bird bird) {
		bird.score += 1;
		if (bird.score > maxScore) {
			maxScore = bird.score;
			scoreText.text = "Score: " + maxScore;
		}
	}

	public void BirdDied() {
		aliveBirds -= 1;
		if (aliveBirds <= 0) {
			gameOver = true;
			centerText.text = GameOverText;
			gamePaused = true;
			// Let provide an addition 1 second for the simulation to run to show the bird
			// fall after hit and only than pause the simulation		
			Invoke("PauseTime", 1);
		}
	}

	void PauseTime() {
		if (gamePaused==true)
			Time.timeScale = 0;
	}
}
