using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Genetic {
	int maxUnits; // total number of units in the population
	int topUnits; // the number of units in the population that don't change from one generation to another
	int randomUnits; // the number of units in the population that will be generated randomly every generation

	public int generation = 1;
	float mutateRate = 0.2f;
	public float bestFitness = -float.MaxValue ;
	
	Mutation[] population;
	Func<Mutation> createNewMutation;

	System.Random randGen;

	public Genetic(int maxUnits, float topUnitsPercentage, float randomUnitsPercentage, Func<Mutation> createNewMutationFunc) {
		this.maxUnits = maxUnits;
		float topUnitsF = (float)maxUnits * topUnitsPercentage;
		this.topUnits = (int)topUnitsF;
		float randomUnitsF = (float)maxUnits * randomUnitsPercentage;
		this.randomUnits = (int)randomUnitsF;
		createNewMutation = createNewMutationFunc;
		CreatePopulation();

		randGen = new System.Random();
	}

	void CreatePopulation() {

		population = new Mutation[maxUnits];

		for (int i = 0; i < maxUnits; i++) {
			population[i] = createNewMutation();	
		}
	}

	public Mutation GetMutation(int index) {
		return population[index];
	}

	public float[] ActivateBrain(int index, float[] inputs) {

		return population[index].Activate(inputs);
	}

	public void SetFitness(int index, float fitness) {
		if (index>=maxUnits) return;
		population[index].fitness = fitness;
	}

	public int GetGeneration(int index) { 
		return population[index].generation;
	}

	public void EvolvePopulation() {
		generation += 1;
		SortByFitness();
		
		Mutation parentA, parentB, offSpring;
		for (int i = topUnits; i < (maxUnits-randomUnits); i++) {
			if (i==topUnits) {
				parentA = population[0];
				parentB = population[1];
			} else if (i<maxUnits-2) {
				parentA = population[randGen.Next(topUnits)];
				parentB = population[randGen.Next(topUnits)];
			} else {
				parentB = parentA = population[randGen.Next(topUnits)];
			}

			if (parentA!=parentB) {
				offSpring = CrossOver( new Mutation[2]{ parentA, parentB });
			} else {
				offSpring = parentA;
			}
			offSpring.generation = Mathf.Max(parentA.generation, parentB.generation) + 1;

			offSpring = Mutate(offSpring, mutateRate);
			population[i] = offSpring;
		}

		for (int i = (maxUnits-randomUnits); i < maxUnits ; i++) {
			population[i] = createNewMutation();
		}

		if (population[0].fitness >bestFitness) {
			bestFitness = population[0].fitness;
		}
		
	}

	void SortByFitness() {
		Array.Sort(population, delegate(Mutation a, Mutation b) { return (int)(b.fitness - a.fitness); }  );
	}

	// this function will randomly increment an index between cur and max, with 15% change to
	// progress to the next index.
	int randInc(int cur, int max) {
		int retCur = cur;
		if (retCur==-1) {
			retCur = randGen.Next(max);
		} else {
			float rnd = (float)randGen.NextDouble();
			if (rnd>0.85f) {
				retCur += 1;
				if (retCur>max) {
					retCur = 0;
				}
			}
		}
		return retCur;
	}

	Mutation CrossOver(Mutation[] parents) {
		Mutation offspring = createNewMutation();
		
		// Copy features from parents, deciding which parent based on the 
		// randInc function
		int curParent = randInc(-1,parents.Length-1);
		for (int i = 0; i < parents[0].NumOfFeatures(); i++) {
			curParent = randInc(curParent,parents.Length-1);
			offspring.SetFeature(i, parents[curParent].GetFeature(i));
		}

		// Copy connections for parents, deciding which parent based on the 
		// randInc function
		curParent = randInc(-1,parents.Length-1);
		for (int i = 0; i < parents[0].NumOfConnections(); i++) {
			curParent = randInc(curParent,parents.Length-1);
			offspring.SetConnection(i, parents[curParent].GetConnection(i));
		}
		return offspring;
	}

	// This function mutate an offspring based on mutation rate: mr
	Mutation Mutate(Mutation offspring, float mr) {
		for (int i = 0; i < offspring.NumOfFeatures(); i++) {
			offspring.SetFeature(i, FeatureMutate(offspring.GetFeature(i), mr));
		}

		for (int i = 0; i < offspring.NumOfConnections(); i++) {
			offspring.SetConnection(i, FeatureMutate(offspring.GetConnection(i), mr));
		}

		return offspring;
	}

	float FeatureMutate(float feature, float mr) {
		float rnd = (float)randGen.NextDouble();
		//UnityEngine.Debug.Log(rnd);
		if (rnd < mr) {
			// This function creates a mutate factor between -1 to 3 with strong 
			// bias towards -1 to 2 values
			rnd = (float)randGen.NextDouble();
			float rnd2 = (float)randGen.NextDouble();
			float mutateFactor = 1 + ((rnd-0.5f) * 3  + (rnd2 - 0.5f));
			//UnityEngine.Debug.Log(mutateFactor);
			feature *= mutateFactor;
		}
		return feature;
	}
}
