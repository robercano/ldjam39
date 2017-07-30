using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    InHold,
    InGame,
}

public class GameManager : MonoBehaviour {

    public GameState gameState;

	private Fortress[] fortressScripts;

	void Start () {
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
		if (fortressScripts.Length == 0) {
			// Draw condition
			Debug.Log ("All players are DEAD!!!");
		} else if (fortressScripts.Length == 1) {
			Debug.Log ("Player " + fortressScripts [0].gameObject.tag + " WINS!!!!");
		}
	}

}
