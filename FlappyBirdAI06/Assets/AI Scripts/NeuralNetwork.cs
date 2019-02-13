using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Neuron {
	public enum NormalizedMethod
	{
		SigmoidMinus1To1 = 0,
		SigmoidZeroTo1 = 1,
		HyperTan = 2
	}

	public float SigmoidMinus1To1(float x) {
		return (2 / (1 + (float)Math.Exp(-2 * x))) - 1;
	}

	static public float SigmoidZeroTo1(float x) {
		return 1 / (1 + (float)Math.Exp(-x));
	}

	static float HyperTanFunction(float x) {
        if (x < -20.0) return -1.0f;
        else if (x > 20.0) return 1.0f;
        else return (float)Math.Tanh(x);
    }
	float[] weights = null;
	float bias = 0;
	float featureRange = 0;
	float lastResult = 0;
	NormalizedMethod method;

	public float GetBias() { return bias; }
	public void SetBias(float bias) {
		this.bias = Mathf.Clamp(bias, -featureRange, featureRange);
	}

	public float GetWeight(int index) { return weights[index]; }
	public void SetWeight(int index, float weight) {
		this.weights[index] = Mathf.Clamp(weight, -featureRange, featureRange);
	}
	public int GetNumWeights() { return weights.Length; }

	public float GetLastResult() { return lastResult; }

	public Neuron(int numOfInputs, NormalizedMethod method) {
		this.method = method;
		if (numOfInputs==0) return;
		
		weights = new float[numOfInputs];
		
		float from = 1.0f / (float)Math.Sqrt(numOfInputs);
		
		for (int i = 0; i < numOfInputs; i++) {
			weights[i] = UnityEngine.Random.Range(-from, from);
		}

		bias = UnityEngine.Random.Range(-from, from); 
		featureRange = from*5;
	}

	public float Activate(float[] inputs) {
		float result = 0;
		for (int i = 0; i < weights.Length; i++) {
			result += inputs[i] * weights[i];
		}
		result += bias;

		float resultM = 0;
		switch (method)
		{
		case NormalizedMethod.SigmoidMinus1To1:
			resultM = SigmoidMinus1To1(result);
			break;
		case NormalizedMethod.SigmoidZeroTo1:
			resultM = SigmoidZeroTo1(result);
			break;
		case NormalizedMethod.HyperTan:
			resultM = HyperTanFunction(result);
			break;
		}
		lastResult = resultM;
		return resultM;
	}
}

public class PreceptorLayer {
	public Neuron[] preceptors;
	public int numOfPreceptors;

	public PreceptorLayer(int numOfPreceptors, int prevLayerNumOfPreceptors, Neuron.NormalizedMethod method) {
		this.numOfPreceptors = numOfPreceptors;
		preceptors = new Neuron[numOfPreceptors];
		for (int i = 0; i < numOfPreceptors; i++) {
			preceptors[i] = new Neuron(prevLayerNumOfPreceptors, method);
		}
	}
	public float[] Activate(float[] inputs) {
		float[] results = new float[numOfPreceptors];
		for (int i = 0; i < numOfPreceptors; i++) {
			results[i] = preceptors[i].Activate(inputs);
		}
		return results;
	}
}

public class NeuralNetwork : Mutation
{
	struct FeatureIndex {
		public int layerNum;
		public int preceptorNum;
		public FeatureIndex(int ln, int pn) {
			layerNum = ln;
			preceptorNum = pn;
		}
	}

	struct ConnectionIndex {
		public int layerNum;
		public int preceptorNum;
		public int weightNum;
		public ConnectionIndex(int ln, int pn, int wn) {
			layerNum = ln;
			preceptorNum = pn;
			weightNum = wn;
		}
	}

    int numOfInputs;
    int[] numOfHiddens;
    int numOfOutputs;

	PreceptorLayer[] layers; // including output
	int numOfLayers;

	int numOfFeatures = 0;
	int numOfConnections = 0;

	FeatureIndex[] featureIndex;
	ConnectionIndex[] connectionIndex;

    public NeuralNetwork(int numOfInputs, int numOfOutputs, 
		int[] numOfHiddens ) : base() {

        this.numOfHiddens = numOfHiddens;
        this.numOfInputs = numOfInputs;
        this.numOfOutputs = numOfOutputs;

		numOfLayers = this.numOfHiddens.Length + 1;

		layers = new PreceptorLayer[numOfLayers];
		int prevNumOfPreceptors = this.numOfInputs;
		for (int i = 0; i < numOfLayers; i++) {
			int numOfPreceptors = i<(numOfLayers-1) ? this.numOfHiddens[i] : this.numOfOutputs;
			Neuron.NormalizedMethod method = i==(numOfLayers-1) ? Neuron.NormalizedMethod.SigmoidZeroTo1 : Neuron.NormalizedMethod.HyperTan;
			layers[i] = new PreceptorLayer(numOfPreceptors, prevNumOfPreceptors, method);

			numOfFeatures += numOfPreceptors;
			numOfConnections += prevNumOfPreceptors * numOfPreceptors;

			prevNumOfPreceptors = numOfPreceptors;
		}

		int idx = 0;
		featureIndex = new FeatureIndex[numOfFeatures];
		for (int i = 0; i < numOfLayers; i++) {
			for (int j = 0; j < layers[i].numOfPreceptors; j++) {
				featureIndex[idx] = new FeatureIndex(i,j);
				idx += 1;
			}
		}

		idx = 0;
		connectionIndex = new ConnectionIndex[numOfConnections];
		for (int i = 0; i < numOfLayers; i++) {
			for (int j = 0; j < layers[i].numOfPreceptors; j++) {
				for (int w = 0; w < layers[i].preceptors[j].GetNumWeights(); w++) {
					connectionIndex[idx] = new ConnectionIndex(i,j,w);
					idx += 1;
				}
			}
		}
    }

    public override float[] Activate(float[] inputs) {
        float[] prevInputs = inputs;
		float[] results = null;
		for (int i = 0; i < numOfLayers; i++) {
			results = layers[i].Activate(prevInputs);
			prevInputs = results;
		}
		return results;
    }

    public override int NumOfFeatures()
    {
        return numOfFeatures;
    }
    public override float GetFeature(int index)
    {
        return layers[ featureIndex[index].layerNum ].preceptors[ featureIndex[index].preceptorNum ].GetBias();
    }
    public override void SetFeature(int index, float feature)
    {
		layers[ featureIndex[index].layerNum ].preceptors[ featureIndex[index].preceptorNum ].SetBias(feature);
    }

    public override int NumOfConnections()
    {
        return numOfConnections;
    }
    public override float GetConnection(int index)
    {
        return layers[ connectionIndex[index].layerNum ].preceptors[ connectionIndex[index].preceptorNum ].GetWeight(connectionIndex[index].weightNum);
    }
    public override void SetConnection(int index, float connection)
    {
        layers[ connectionIndex[index].layerNum ].preceptors[ connectionIndex[index].preceptorNum ].SetWeight(connectionIndex[index].weightNum, connection);
    }

}
