using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	Camera camera;
	int currentWidth = 0;
	int currentHeight = 0;

	void Start () {
		camera = GetComponent<Camera> ();
		currentWidth = Screen.width;
		currentHeight = Screen.height;
		UpdateCameraSize ();
	}

	void Update () {
		int width = Screen.width;
		int height = Screen.height;

		if (currentWidth != width ||
			currentHeight != height) {
			currentWidth = width;
			currentHeight = height;
			UpdateCameraSize ();
		}
	}

	void UpdateCameraSize ()
	{
		camera.orthographicSize = 32.0f * currentHeight / currentWidth / 2.0f;
	}
}
