using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBatteryManager : MonoBehaviour {

	public GameObject batteryPrefab;
	public int batteriesToGenerate = 3;
	public int energyPerBattery = 20;
	public float heightMultiplier = 2.0f;

	private List<GameObject> currentBatteries;
	private List<GameObject> playersWithBatteries;
	private int stageLayerID;

	void Awake () {
		currentBatteries = new List<GameObject>();
		playersWithBatteries = new List<GameObject>();
		stageLayerID = LayerMask.NameToLayer ("Stage");
	}

	void Start () {
	}

	void Update () {
		CheckRespawnBatteries ();
	}

	public void NotifyPlayerDroppedBattery(GameObject player, PlayerMovement playerMovementScript) {
		Vector3 newPosition = GetPositionToDropBattery (player, playerMovementScript);
		Vector3 forceToDropBattery;
		if (newPosition == playerMovementScript.GetPositionInFront ()) {
			// Add the player's speed if we are throwing it in his running direction 
			forceToDropBattery = (1000.0f * playerMovementScript.GetFrontDirection ()) + (100.0f * playerMovementScript.GetPlayerSpeed ());	
		} else {
			forceToDropBattery = (1000.0f * playerMovementScript.GetBackDirection ());	
		}

		GameObject droppedBattery = Instantiate (batteryPrefab, newPosition, Quaternion.identity);
		Rigidbody droppedBatteryRB = droppedBattery.GetComponent<Rigidbody> ();
		droppedBatteryRB.AddForce(forceToDropBattery.x, forceToDropBattery.y, forceToDropBattery.z, ForceMode.Impulse);

		playersWithBatteries.Remove (player);
		currentBatteries.Add (droppedBattery);
	}

	// It will check if putting it on front will throw it out of the level, id so, will put in the back
	public Vector3 GetPositionToDropBattery(GameObject player, PlayerMovement playerMovementScript) {
		float maxDistanceToThrowBoxesOut = 2f;
		int layerMask = 1<< stageLayerID;
		if (Physics.Raycast (player.transform.position, playerMovementScript.GetFrontDirection (), maxDistanceToThrowBoxesOut, layerMask)) {
			return playerMovementScript.GetPositionInBack ();
		} else {
			return playerMovementScript.GetPositionInFront ();
		}

	}

	public void NotifyPlayerPickedBattery(GameObject player, GameObject battery) {
		playersWithBatteries.Add (player);
		currentBatteries.Remove (battery);
	}

	public bool NotifyPlayerUsedBattery(GameObject player, GameObject fortress) {
		if (AddEnergyToFortress(energyPerBattery, fortress)) {
			playersWithBatteries.Remove (player);
			return true;
		}
		return false;
	}

	public bool NotifyFortressGotBattery(GameObject fortress, GameObject battery) {
		if (AddEnergyToFortress(energyPerBattery, fortress)) {
			currentBatteries.Remove (battery);
			return true;
		}
		return false;
	}

	private bool AddEnergyToFortress(int energy, GameObject fortress) {
		Fortress fortressScript = fortress.GetComponent ("Fortress") as Fortress;
		return fortressScript.AddEnergy (energyPerBattery);
	}

	private void CheckRespawnBatteries() {
		if (!AreThereBatteries()) {
			GenerateBatteries ();
		}
	}

	private bool AreThereBatteries() {
		return AreThereBatteriesInTheField() || AreTherePlayersCarryingBatteries();
	}

	private bool AreThereBatteriesInTheField() {
		GameObject[] batteryArray = currentBatteries.ToArray ();
		if (batteryArray.Length == 0) {
			return false;
		}
		for (int i = 0; i < batteryArray.Length; i++) {
			if ((batteryArray [i] != null) && (batteryArray [i].activeInHierarchy)) {
				return true;
			}
		}
		return false;
	}

	private bool AreTherePlayersCarryingBatteries() {
		return playersWithBatteries.ToArray ().Length > 0;
	}

	private void GenerateBatteries() {
		GameObject[] batteriesSpawnTiles = GameObject.FindGameObjectsWithTag (LevelManager.BatterySpawnTileGameTag);

		currentBatteries = new List<GameObject>();
		for (int i = 0; i < batteriesSpawnTiles.Length; i++) {
			GameObject newBattery = Instantiate (batteryPrefab, new Vector3(
				batteriesSpawnTiles[i].transform.position.x,
				LevelManager.AboveFloorHeight * heightMultiplier,
				batteriesSpawnTiles[i].transform.position.z
			), Quaternion.identity);
			currentBatteries.Add (newBattery);
		}
	}
}
