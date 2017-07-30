using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fortress : MonoBehaviour {

	// Getters
	public int CurrentEnergy {
		get {
			return this.currentEnergy;
		}
	}

	// Inspector variables
	[Header("Fortress Energy Settings")]
	public int initialEnergy = 100;
	public int maxEnergy = 150;

	[Tooltip("Time in seconds")]
	public float energyDecayTime = 5.0f; // In seconds

	public int energyDecayAmount = 5;    // In energy units

	private int currentEnergy = 0;
	private float lastDecayTime = 0.0f; 
	private int batteryLayerID;
	private SceneBatteryManager sceneBatteryManager;
    private GameManager gameManager;
	private AudioSource audioSource;
	public AudioClip receiveEnergySound;
	public AudioClip lowBatterySound;

    #region - Public methods
    public bool AddEnergy(int energy)
	{
		audioSource.PlayOneShot (receiveEnergySound);
		currentEnergy += energy;
		if (currentEnergy > maxEnergy) {
			currentEnergy = maxEnergy;
		}

		return true;
	}
	#endregion // Public methods

	#region - Private methods 

	void Awake () {
		currentEnergy = initialEnergy;
		batteryLayerID = LayerMask.NameToLayer ("Battery");
        gameManager = GameObject.Find("Level").GetComponent<GameManager>();

        lastDecayTime = Time.time;
	}

	void Start () {
		sceneBatteryManager = GameObject.Find("Level").GetComponent<SceneBatteryManager>();
		audioSource = GetComponent<AudioSource>();
	}

	void Update () {
        if (gameManager.gameState != GameState.InGame)
            return;

        UpdateEnergyDecay ();
		UpdateEnergyBar ();
	}

	public float GetPercentage() {
		return (float)currentEnergy / (float)maxEnergy;
	}

	void UpdateEnergyDecay()
	{
		if (currentEnergy <= 0) {
			return;
		}

		float currentTime = Time.time;

		// Check if we need to decay the energy
		if (currentTime - lastDecayTime >= energyDecayTime) {
			lastDecayTime = currentTime;

			currentEnergy -= energyDecayAmount;
			if (GetPercentage() < 0.2f) {
				audioSource.PlayOneShot (lowBatterySound);
			}
			if (currentEnergy < 0) {
				currentEnergy = 0;
			}

//			Debug.Log (gameObject.tag + " fortress energy is " + currentEnergy);
		}
	}

	private void UpdateEnergyBar() {
		// TODO: update sprite
	}

	void OnCollisionEnter (Collision col) {
		if (col.gameObject.layer == batteryLayerID) {
			HandleCollisionWithBattery (col.gameObject);
		}
	}

	void HandleCollisionWithBattery(GameObject battery) {
		if (sceneBatteryManager.NotifyFortressGotBattery (gameObject, battery)) {
			Destroy(battery);
		}
	}
	#endregion // Private methods
}
