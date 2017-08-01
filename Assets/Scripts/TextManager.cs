using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TextManager : MonoBehaviour {

    private GameManager gameManager;
    private Text text;
	public AudioClip countDownSound;

	// Use this for initialization
	void Awake () {
        text = GameObject.Find("Text").GetComponent<Text>();
        gameManager = GameObject.Find("Level").GetComponent<GameManager>();
    }

    void Start()
    {
        StartCoroutine(Countdown());
    }

    public void End(string endText)
    {
        StartCoroutine(EndText(endText));
    }

    IEnumerator Countdown()
    {
        gameManager.gameState = GameState.InHold;

        text.text = "3";
		AudioSource.PlayClipAtPoint (countDownSound, Camera.main.transform.position);
        yield return new WaitForSeconds(1f);

        text.text = "2";
		AudioSource.PlayClipAtPoint (countDownSound, Camera.main.transform.position);
        yield return new WaitForSeconds(1f);

        text.text = "1";
		AudioSource.PlayClipAtPoint (countDownSound, Camera.main.transform.position);
        yield return new WaitForSeconds(1f);

        text.text = "FIGHT!";
        yield return new WaitForSeconds(1f);

        text.text = "";
        
        gameManager.gameState = GameState.InGame;
		yield return null;
    }

    IEnumerator EndText(string endText)
    {
        gameManager.gameState = GameState.InHold;

        text.text = endText;
        yield return new WaitForSeconds(4f);

        text.text = "";
        gameManager.gameState = GameState.EndGame;
    }
}
