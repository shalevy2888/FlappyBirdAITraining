using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour {
	private static GameLogic instance = null;
	public static GameLogic GetInstance() { return instance; }
	private bool gameOver = false;
	public Text scoreText;
	public Text generationText;
	List<Bird> birds = new List<Bird>();
	public GameObject birdPrefab;
	
	[Header("Neural Network Properties")]
	public int numOfUnitsInGeneration = 40;
	public float topUnitsPercentage = 0.25f;
	public float randomUnitsPercentage = 0.10f;
	public int maxScorePerGeneration = 10;
	int generationNum = 0;
	float distance;
	Genetic genetic;
	Mutation CreateNetwork() {
		return new NeuralNetwork(2, 1, new int[1]{6});
	}
	ColumnSpawn columnSpawn; // To be able to call GetNearestColumn() and Restart() functions
	
	int maxScore;
	int aliveBirds;

	// Use this for initialization
	void InstantiateBirds() {
		for (int i = 0; i < numOfUnitsInGeneration; i++) {
			Bird bird = Instantiate(birdPrefab, Vector2.zero, Quaternion.identity).GetComponent<Bird>();
			birds.Add(bird);
			bird.mutationIndex = i; 
		}
	}
	
	void Awake() {
		instance = this;
	}
	void Start () {
		columnSpawn = GetComponent<ColumnSpawn>(); 
		InstantiateBirds();
		genetic = new Genetic(numOfUnitsInGeneration, topUnitsPercentage, randomUnitsPercentage, CreateNetwork );

		Restart();
	}

	void Restart() {
		gameOver = false;
		maxScore = 0;
		aliveBirds = numOfUnitsInGeneration;
		scoreText.text = "Score: 0";
		// Restart game:
		// 1. restart Bird:
		foreach (var bird in birds)
		{
			bird.Restart();
		}
		columnSpawn.Restart();
	}
	// Update is called once per frame
	void Update () {
		if (!gameOver) {
			distance += Time.deltaTime;
			for (int i = 0; i < numOfUnitsInGeneration; i++) {
				if (birds[i].isDead == false) {
					float[] inputs = new float[2]; // prepare the genetic activate function inputs
					// Will set inputs to the distance in x and height difference in y to the nearest column
					columnSpawn.GetNearestColumn(birds[i].transform, out inputs[0], out inputs[1]);
					float[] result = genetic.ActivateBrain(i, inputs);
					if (result[0] > 0.5f) { // treat results greater than 0.5 as true
						birds[i].flapOnNextFrame = true;
					}
				}
			}
		} else {
			// Game is over, either all the population has died or game ended with max score reached
			// In any case, evolve the population and run the next generation
			genetic.EvolvePopulation();
			generationNum += 1;
			generationText.text = "Generation: " + generationNum;
			//generationText.text = "Generation: " + generationNum.ToString();
			Restart();
		}
	}

	public void BirdScored(Bird bird) {
		bird.score += 1;
		if (bird.score > maxScore) {
			maxScore = bird.score;
			scoreText.text = "Score: " + maxScore;

			// Optional code: end the game if max score was reached
			if (maxScore >= maxScorePerGeneration) {
				// Force a new generation, but make sure to update 
				// all the birds with their fitness score.
				foreach (var bird1 in birds)
				{	
					if (bird.isDead == false) {
						BirdDied(bird1);
					}
				}
			}
		}
	}

	public void BirdDied(Bird bird) {
		// Set the bird fitness score, to be used by the genetic algorithm
		// when sorting the birds by their individual fitness scores
		genetic.SetFitness(bird.mutationIndex, distance);

		aliveBirds -= 1;
		if (aliveBirds <= 0) {
			gameOver = true;
		}
	}
}
