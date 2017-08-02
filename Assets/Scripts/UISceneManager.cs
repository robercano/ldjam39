using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UISceneManager : MonoBehaviour {

    private void Start() {
        Cursor.visible = true;
        LevelManager.CurrentLevel = 0;
    }

    public void GoToControllerMenu() {
		SceneManager.LoadScene("ControllerMenu");
	}

	public void GoToMainMenu() {
		SceneManager.LoadScene("MainMenu");
	}

	public void GoToCreditsMenu() {
		SceneManager.LoadScene("Credits");
	}

	public void ExitGame() {
		Application.Quit();
	}
}
