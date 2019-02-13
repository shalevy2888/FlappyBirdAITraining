using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mutation  {
	public float fitness;
	public int generation;
	
	public Mutation() {
		fitness = 0;
		generation = 1;
	}

	public abstract float[] Activate(float[] inputs);

	public abstract int NumOfFeatures();
	public abstract float GetFeature(int index);
	public abstract void SetFeature(int index, float feature);

	public abstract int NumOfConnections();
	public abstract float GetConnection(int index);
	public abstract void SetConnection(int index, float connection);

}
