using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour {

    private GameManager gameManager;
    private Text text;

	// Use this for initialization
	void Awake () {
        text = GameObject.Find("Text").GetComponent<Text>();
        gameManager = GameObject.Find("Level").GetComponent<GameManager>();
    }

    void Start()
    {
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        gameManager.gameState = GameState.InHold;

        text.text = "3";
        yield return new WaitForSeconds(1f);

        text.text = "2";
        yield return new WaitForSeconds(1f);

        text.text = "1";
        yield return new WaitForSeconds(1f);

        text.text = "FIGHT!";
        yield return new WaitForSeconds(1f);

        text.text = "";
        gameManager.gameState = GameState.InGame;
    }
}
