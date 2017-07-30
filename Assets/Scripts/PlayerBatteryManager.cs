using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBatteryManager : MonoBehaviour {
	public GameObject batteryPrefab;
	public AudioClip dropBatterySound;
	public AudioClip pickBatterySound;

	private int fortressLayerID;
	private int batteryLayerID;
	private bool hasBattery;
	private PlayerMovement playerMovementScript;
	private SceneBatteryManager sceneBatteryManager;
	private AudioSource audioSource;

	void Awake () {
		hasBattery = false;

		fortressLayerID = LayerMask.NameToLayer ("Fortress");
		batteryLayerID = LayerMask.NameToLayer ("Battery");
	}

	void Start () {
		playerMovementScript = gameObject.GetComponent<PlayerMovement> ();
		sceneBatteryManager = GameObject.Find("Level").GetComponent<SceneBatteryManager>();
		audioSource = GetComponent<AudioSource>();
	}

	public bool HasBattery() {
		return hasBattery;
	}

	public void DropBattery() {
		if (HasBattery ()) {
			sceneBatteryManager.NotifyPlayerDroppedBattery (gameObject, playerMovementScript);
			hasBattery = false;
			audioSource.PlayOneShot (dropBatterySound);
		}
	}

	public void DropBatteryAfterKnockOut(Vector3 directionOfDrop) {
		sceneBatteryManager.DropBatteryAfterKnockout (gameObject, directionOfDrop, playerMovementScript);
		audioSource.PlayOneShot (dropBatterySound);
	}

	void OnCollisionEnter (Collision col) {
		if (col.gameObject.layer == batteryLayerID) {
			HandleCollisionWithBattery (col.gameObject);
		} else if(col.gameObject.layer == fortressLayerID) {
			HandleCollisionWithFortress (col.gameObject);
		}
	}

	private void HandleCollisionWithBattery(GameObject battery) {
		if (!hasBattery) {
			Destroy(battery);
			hasBattery = true;
			sceneBatteryManager.NotifyPlayerPickedBattery (gameObject, battery);
			audioSource.PlayOneShot (pickBatterySound);
		}
	}

	private void HandleCollisionWithFortress(GameObject fortress) {
		if (!hasBattery) {
			return;
		}
		if(fortress.tag != gameObject.tag) {
			return;
		}
		if (sceneBatteryManager.NotifyPlayerUsedBattery (gameObject, fortress)) {
			hasBattery = false;
		}
	}
}
