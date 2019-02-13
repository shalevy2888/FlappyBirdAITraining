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
	public Bird bird;

	private const string WellcomeText = "Flappy Bird, press Return to start.";
	private const string GameOverText = "Game Over, press Return to start.";
	// Use this for initialization
	void Awake() {
		instance = this;
	}
	void Start () {
		centerText.text = WellcomeText;
		Time.timeScale = 0;
		gamePaused = true;
	}
	// Update is called once per frame
	void Update () {
		if (gamePaused == true) {
			if (Input.GetKeyDown(KeyCode.Return)) {
				// Un pause the game either start (first time playing), or restart
				if (gameOver == true) {
					// Restart game:
					// 1. restart Bird:
					bird.Restart();
					GetComponent<ColumnSpawn>().Restart();
				}
				centerText.text = "";
				Time.timeScale = 1;
				gamePaused = false;
			}
		}
	}

	public void BirdScored(Bird bird) {
		bird.score += 1;
		//Debug.Log("Score: " + bird.score);
		scoreText.text = "Score: " + bird.score;
	}

	public void BirdDied() {
		// Let provide an addition 1 second for the simulation to run to show the bird
		// fall after hit and only than pause the simulation		
		gameOver = true;
		centerText.text = GameOverText;
		gamePaused = true;
		Invoke("PauseTime", 1);
	}

	void PauseTime() {
		if (gamePaused==true)
			Time.timeScale = 0;
	}
}
