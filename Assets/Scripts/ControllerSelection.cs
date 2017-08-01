using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControllerSelection : MonoBehaviour {

	private const string Keyboard1Selection = "Keyboard1Selection";
	private const string Controller1Selection = "Controller1Selection";
	private const string CPU2Selection = "CPU2Selection";
	private const string Controller2Selection = "Controller2Selection";

	void Start()
	{
		UnselectEverythingForRed ();
		UnselectEverythingForBlue ();

		ParseGameConfig ();
	}

	public void OnBackClicked()
	{
		SceneManager.LoadScene("MainMenu");
	}
		
	public void OnPlayClicked()
	{
		SceneManager.LoadScene("Level");
	}

	public void SetBluePlayerKeyboard()
	{
		UnselectEverythingForBlue ();
		SelectElement (Keyboard1Selection);
		GameConfig.Instance.SetPlayerController (GameConfig.Team.Blue, GameConfig.ControlType.Keyboard);
	}
	public void SetBluePlayerController()
	{
		UnselectEverythingForBlue ();
		SelectElement (Controller1Selection);
		GameConfig.Instance.SetPlayerController (GameConfig.Team.Blue, GameConfig.ControlType.Controller);
	}
	public void SetRedPlayerController()
	{
		UnselectEverythingForRed ();
		SelectElement (Controller2Selection);
		GameConfig.Instance.SetPlayerController (GameConfig.Team.Red, GameConfig.ControlType.Controller);
	}
	public void SetRedPlayerCPU()
	{
		UnselectEverythingForRed ();
		SelectElement (CPU2Selection);
		GameConfig.Instance.SetPlayerController (GameConfig.Team.Red, GameConfig.ControlType.AI);
	}

	void ParseGameConfig()
	{
		GameConfig.PlayerConfig bluePlayerConfig = GameConfig.Instance.GetPlayerConfig (GameConfig.Team.Blue);

		switch (bluePlayerConfig.controlType) {
		case GameConfig.ControlType.Keyboard:
			SetBluePlayerKeyboard ();
			break;
		case GameConfig.ControlType.Controller:
			SetBluePlayerController ();
			break;
		}

		GameConfig.PlayerConfig redPlayerConfig = GameConfig.Instance.GetPlayerConfig (GameConfig.Team.Red);

		switch (redPlayerConfig.controlType) {
		case GameConfig.ControlType.Controller:
			SetRedPlayerController ();
			break;
		case GameConfig.ControlType.AI:
			SetRedPlayerCPU ();
			break;
		}
	}

	void UnselectEverythingForBlue()
	{
		UnselectElement (Keyboard1Selection);
		UnselectElement (Controller1Selection);
	}

	void UnselectEverythingForRed()
	{
		UnselectElement (CPU2Selection);
		UnselectElement (Controller2Selection);
	}

	void SelectElement(string element)
	{
		GameObject.Find (element).GetComponent<Image> ().color = Color.white;
	}

	void UnselectElement(string element)
	{
		GameObject.Find (element).GetComponent<Image> ().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
	}
}
