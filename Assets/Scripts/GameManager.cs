using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    InHold,
    InGame,
    EndGame,
}

public class GameManager : MonoBehaviour {

    public GameState gameState;
    private LevelManager levelManager;
    private TextManager textManager;
	private Fortress[] fortressScripts;
	private MusicManager musicManager;

    private void Awake()
    {
		levelManager = GameObject.FindObjectOfType<LevelManager> () as LevelManager;
        textManager = GameObject.Find("Text").GetComponent<TextManager>();
		musicManager = GameObject.FindObjectOfType<MusicManager> ();
    }

    void Start () {
		Cursor.visible = false;

		levelManager.LoadNextLevel();
		levelManager.BakeNavMesh();
		levelManager.SpawnPlayers ();

		fortressScripts = GameObject.FindObjectsOfType<Fortress> () as Fortress[];

		musicManager.PlayLevelMusic (levelManager.GetCurrentLevel());
	}

	void Update () {
		UpdateAlivePlayers ();
		CheckVictoryCondition ();
		CheckExit ();
	}

	void UpdateAlivePlayers()
	{
		List<Fortress> aliveFortresses = new List<Fortress>();
		List<Fortress> deadFortresses = new List<Fortress>();

		foreach (Fortress fortress in fortressScripts) {
			if (fortress.CurrentEnergy == 0) {
				deadFortresses.Add (fortress);
			} else {
				aliveFortresses.Add (fortress);
			}
		}

		// TODO: Notify deadFortresses players that they are dead

		fortressScripts = aliveFortresses.ToArray ();
	}

	void CheckVictoryCondition()
	{
        if (gameState == GameState.EndGame){
			if (levelManager.GetCurrentLevel () == LevelManager.MaxLevels) {
				SceneManager.LoadScene("Credits");
			} else {
				SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
			}
        } else if (fortressScripts.Length == 0) {
            textManager.End("DRAW!");
        } else if (fortressScripts.Length == 1) {
            textManager.End(fortressScripts[0].gameObject.tag.ToUpper() + " WINS!");
		}
    }

	void CheckExit() {
		if (Input.GetKey(KeyCode.Escape)) {
			SceneManager.LoadScene("MainMenu");
		}
	}

}
