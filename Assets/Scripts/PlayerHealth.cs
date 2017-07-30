using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {
	public int maxHealth = 100;
	public float knockedOutTime = 2.0f;
	public AudioClip knockedOutSound;
	public AudioClip recoverSound;

	private float timeFromLastKnockOut;
	private bool isKnockedOut = false;
	private Vector3 lastDamageDirection;

	private int currentHealth = 0;
	private PlayerBatteryManager playerBatteryManager;
	private PlayerMovement playerMovementScript;
	private AudioSource audioSource;

	#region - Public methods
	public void ApplyDamage(int damage, Vector3 damageDirection)
	{
		currentHealth -= damage;

		if (currentHealth < 0) {
			currentHealth = 0;
		}
		lastDamageDirection = damageDirection;
	}

	public bool IsKnockedOut() {
		return isKnockedOut;
	}
	#endregion

	#region - Private methods
	void Awake () {
		currentHealth = maxHealth;
		playerBatteryManager = gameObject.GetComponent<PlayerBatteryManager> ();
		playerMovementScript = gameObject.GetComponent<PlayerMovement> ();
	}

	void Start () {
		audioSource = GetComponent<AudioSource>();
	}

	void Update () {
		CheckHealth ();
		CheckKnockedOut ();
	}

	void CheckKnockedOut() {
		if (IsKnockedOut ()) {
			if ((Time.time - timeFromLastKnockOut) >= knockedOutTime) {
				RecoverFromKnockOut ();
			}
		}
	}

	void CheckHealth() {
		if ((currentHealth <= 0) && !IsKnockedOut ()) {
			KnockOut ();
		}
	}

	private void KnockOut() {
		timeFromLastKnockOut = Time.time;
		isKnockedOut = true;
		playerMovementScript.StopPlayer ();
		audioSource.PlayOneShot (knockedOutSound);
		if (playerBatteryManager.HasBattery ()) {
			playerBatteryManager.DropBatteryAfterKnockOut (lastDamageDirection);
		}
	}

	private void RecoverFromKnockOut() {
		isKnockedOut = false;
		currentHealth = maxHealth;
		audioSource.PlayOneShot (recoverSound);
	}
	#endregion // Private methods
}
