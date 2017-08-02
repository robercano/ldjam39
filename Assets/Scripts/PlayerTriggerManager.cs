using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerManager : MonoBehaviour {

    private PlayerBatteryManager playerBatteryManager;

    // Use this for initialization
    void Start () {
        playerBatteryManager = transform.parent.gameObject.GetComponent<PlayerBatteryManager>();
    }

    void OnTriggerEnter(Collider col)
    {
        playerBatteryManager.OnBatteryTrigger(col);
    }
}
