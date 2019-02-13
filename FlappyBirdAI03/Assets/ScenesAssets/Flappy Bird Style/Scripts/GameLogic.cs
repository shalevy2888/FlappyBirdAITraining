using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour {
	private static GameLogic instance = null;
	public static GameLogic GetInstance() { return instance; }
	private bool gameOver = false;
	public bool GameOver {
		get { return gameOver; }
	}
	public Text scoreText;
	public Text gameOverText;
	// Use this for initialization
	void Awake() {
		instance = this;
	}
	void Start () {
		
	}
	// Update is called once per frame
	void Update () {
		
	}

	public void BirdScored(Bird bird) {
		bird.score += 1;
		//Debug.Log("Score: " + bird.score);
		scoreText.text = "Score: " + bird.score;
	}

	public void BirdDied() {
		gameOver = true;
		gameOverText.text = "Game Over";
	}
}
