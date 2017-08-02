using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBatteryManager : MonoBehaviour {
	public GameObject batteryPrefab;
	public AudioClip dropBatterySound;
	public AudioClip pickBatterySound;
    public float pickCooldown = 0.3f;

    private int fortressLayerID;
	private int batteryLayerID;
	private bool hasBattery;
	private PlayerMovement playerMovementScript;
	private SceneBatteryManager sceneBatteryManager;
	private PlayerFistAttack playerFistAttackScript;
	private AudioSource audioSource;
    private float pickTimer;

	void Awake () {
		hasBattery = false;

		fortressLayerID = LayerMask.NameToLayer ("Fortress");
		batteryLayerID = LayerMask.NameToLayer ("Battery");
	}

	void Start () {
		playerMovementScript = gameObject.GetComponent<PlayerMovement> ();
		sceneBatteryManager = GameObject.Find("Level").GetComponent<SceneBatteryManager>();
		playerFistAttackScript = GetComponent<PlayerFistAttack> ();
		audioSource = GetComponent<AudioSource>();

		playerFistAttackScript.SetCarryBatteryEnabled (false);
	}

    private void Update()
    {
        //Update timer cooldown
        if (pickTimer > 0f)
            pickTimer -= Time.deltaTime;
    }

    public bool HasBattery() {
		return hasBattery;
	}

	public void DropBattery() {
		if (HasBattery ()) {
			sceneBatteryManager.NotifyPlayerDroppedBattery (gameObject, playerMovementScript);
			hasBattery = false;
			playerFistAttackScript.SetCarryBatteryEnabled (false);
			audioSource.PlayOneShot (dropBatterySound);
            pickTimer = pickCooldown;
		}
	}

	public void DropBatteryAfterKnockOut(Vector3 directionOfDrop) {
		sceneBatteryManager.DropBatteryAfterKnockout (gameObject, directionOfDrop, playerMovementScript);
		hasBattery = false;
		playerFistAttackScript.SetCarryBatteryEnabled (false);
		audioSource.PlayOneShot (dropBatterySound);
        pickTimer = pickCooldown;
    }

	public void OnBatteryTrigger (Collider col) {
		if (col.gameObject.layer == batteryLayerID) {
			HandleCollisionWithBattery (col.gameObject);
		} else if(col.gameObject.layer == fortressLayerID) {
			HandleCollisionWithFortress (col.gameObject);
		}
	}

	private void HandleCollisionWithBattery(GameObject battery) {
        //Battery pick is on cooldown
        if (pickTimer > 0f)
            return;

		if (!hasBattery && !sceneBatteryManager.IsBatteryFromSomebody(battery)) {
			Destroy(battery);
			hasBattery = true;
			playerFistAttackScript.SetCarryBatteryEnabled (true);
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
			playerFistAttackScript.SetCarryBatteryEnabled (false);
		}
	}
}
