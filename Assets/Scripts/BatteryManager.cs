using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryManager : MonoBehaviour {

    private SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Start () {
        spriteRenderer = this.transform.Find("Sprite").GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        spriteRenderer.sortingOrder = (int)this.transform.position.z * -100;
    }
}
