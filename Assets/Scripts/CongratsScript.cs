using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CongratsScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (LevelManager.CurrentLevel == LevelManager.MaxLevels) {
			GetComponent<Text> ().enabled = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
