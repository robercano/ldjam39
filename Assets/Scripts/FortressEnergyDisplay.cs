using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortressEnergyDisplay : MonoBehaviour {

	private Fortress fortressScript;
	private SpriteRenderer spriteRendererEnergyDisplay;

	public Sprite[] spritesStates;

	void Start () {
		fortressScript = gameObject.GetComponent<Fortress> ();
		spriteRendererEnergyDisplay = gameObject.transform.Find ("Energy").GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	void Update () {
		int spriteIndexToUse = (int) Mathf.Floor( (1 - fortressScript.GetPercentage()) * (float)spritesStates.Length);
		spriteIndexToUse = Mathf.Clamp (spriteIndexToUse, 0, spritesStates.Length - 1);
		spriteRendererEnergyDisplay.sprite = spritesStates [spriteIndexToUse];
	}
}
