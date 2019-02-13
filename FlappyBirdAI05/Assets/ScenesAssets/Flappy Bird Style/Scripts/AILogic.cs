using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILogic {
	public bool FlapWings(float param1, float param2) {
		if (param2 < 0 && UnityEngine.Random.Range(0,100)>90f) {
			return true;
		}
		if (param2 >= 0 && UnityEngine.Random.Range(0,100)>97f) {
			return true;
		}
		return false;
	}
}
