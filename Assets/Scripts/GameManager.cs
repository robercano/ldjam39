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

    private void Awake()
    {
		levelManager = GameObject.FindObjectOfType<LevelManager> () as LevelManager;
        textManager = GameObject.Find("Text").GetComponent<TextManager>();
    }

    void Start () {
        Cursor.visible = false;

		levelManager.LoadNextLevel ();

        fortressScripts = GameObject.FindObjectsOfType<Fortress> () as Fortress[];
	}

	void Update () {
		UpdateAlivePlayers ();
		CheckVictoryCondition ();
	}

	void UpdateAlivePlayers()
	{
		List<Fortress> aliveFortresses = new List<Fortress>();
		List<Fortress> deadFortresses = new List<Fortress>();

		foreach (Fortress fortress in fortressScripts) {
			if (fortress.CurrentEnergy == 0) {
				Debug.Log ("Fortress " + fortress.gameObject.tag + "has lost!!!");

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
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } else if (fortressScripts.Length == 0) {
            textManager.End("DRAW!");
        } else if (fortressScripts.Length == 1) {
            textManager.End(fortressScripts[0].gameObject.tag.ToUpper() + " WINS!");
		}
    }

}
