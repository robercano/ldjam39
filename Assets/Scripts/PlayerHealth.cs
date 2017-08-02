using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {
	public int maxHealth = 100;
	public float knockedOutTime = 2.0f;
    public float invulnerabilityTime = 4.0f;
    public float invulnerabilityAlpha = 0.6f;
    public AudioClip knockedOutSound;
	public AudioClip recoverSound;

	private float timeFromLastKnockOut;
	private bool isKnockedOut = false;
	private Vector3 lastDamageDirection;

	private int currentHealth = 0;
	private PlayerBatteryManager playerBatteryManager;
	private PlayerMovement playerMovementScript;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private Animator animator;

    #region - Public methods
    public void ApplyDamage(int damage, Vector3 damageDirection) {
		currentHealth -= damage;

		if (currentHealth < 0) {
			currentHealth = 0;
		}
		lastDamageDirection = damageDirection;
	}

	public bool IsKnockedOut() {
		return isKnockedOut;
	}

    public bool IsInvulnerable() {
        return ((Time.time - timeFromLastKnockOut) < invulnerabilityTime);   
    }
    #endregion

    #region - Private methods
    void Awake () {
		currentHealth = maxHealth;
		playerBatteryManager = gameObject.GetComponent<PlayerBatteryManager> ();
		playerMovementScript = gameObject.GetComponent<PlayerMovement> ();
        GameObject sprite = transform.Find("Sprite").gameObject;
        spriteRenderer = sprite.GetComponent<SpriteRenderer>();
        animator = sprite.GetComponent<Animator>();
}

	void Start () {
		audioSource = GetComponent<AudioSource>();
	}

	void Update () {
		CheckHealth ();
		CheckKnockedOut ();
        CheckInvulnerability();
	}

    void CheckInvulnerability() {
        if (IsInvulnerable()) {
            if (spriteRenderer.color.a == 1f)
                spriteRenderer.color = new Color(1f, 1f, 1f, invulnerabilityAlpha);
        } else {
            if (spriteRenderer.color.a != 1f)
                spriteRenderer.color = Color.white;
        }
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
        animator.SetInteger("State", (int)PlayerState.Stun);
        if (playerBatteryManager.HasBattery ()) {
			playerBatteryManager.DropBatteryAfterKnockOut (lastDamageDirection);
		}
	}

	private void RecoverFromKnockOut() {
		isKnockedOut = false;
		currentHealth = maxHealth;
		audioSource.PlayOneShot (recoverSound);
        animator.SetInteger("State", (int)PlayerState.Idle);
    }
	#endregion // Private methods
}
