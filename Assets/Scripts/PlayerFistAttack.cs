using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class PlayerFistAttack : MonoBehaviour {
	public int damage = 1;
	public float forwardFistDistance = 1.0f;
	public float lateralFistDistance = 1.0f;
	public float batteryDistance = 1.0f;
	public GameObject fistPrefab;
	public GameObject batteryPrefab;
	public GameObject crosshairPrefab;

	public void SetInputEnabled(bool enable)
	{
		isInputEnabled = enable;
	}

	public void SetTarget(Vector3 target)
	{
		this.target = target;
		updateTarget = true;
	}

	public Vector3 GetDirection() {
		return this.target;
	}

	public bool canShoot()
	{
		if (leftFistScript.GetState () == Fist.State.Idle ||
			rightFistScript.GetState () == Fist.State.Idle) {
			return true;
		} else {
			return false;
		}
	}

	public void Shoot()
	{
		isShootButtonPressed = true;
	}

	public void SetCarryBatteryEnabled(bool enable)
	{
		battery.SetActive (enable);
	}

    void Awake()
	{
        gameManager = GameObject.Find("Level").GetComponent<GameManager>();
		batteryManager = gameObject.GetComponent<PlayerBatteryManager> ();
		InstantiateBattery ();
    }

    void Start () {
		InstantiateCrosshair ();
		InstantiateFists ();
	
		playerHealthScript = gameObject.GetComponent<PlayerHealth> ();
        GameObject sprite = transform.Find("Sprite").gameObject;
        spriteRenderer = sprite.GetComponent<SpriteRenderer>();
        animator = sprite.GetComponent<Animator>();
    }

	void Update () {
		if (gameManager.gameState != GameState.InGame) {
			return;
		}

		if (isInputEnabled) {
			ReadInput ();
		}
		if (updateTarget) {
			UpdateFistsPosition ();
			UpdateBatteryPosition ();
			updateTarget = false;
		}

		if (isShootButtonPressed) {
			OnShootButtonPressed ();
			isShootButtonPressed = false;
		}

        UpdateAnimation();
	}

	void InstantiateBattery ()
	{
		battery = Instantiate (batteryPrefab, transform);
		battery.SetActive (false);
	}

	void InstantiateCrosshair ()
	{
		GameObject ui = GameObject.Find ("UI");

		crosshair = Instantiate (crosshairPrefab, ui.transform);
		crosshair.name = gameObject.tag + "Crosshair";
		crosshair.tag = gameObject.tag;

		crosshairScript = crosshair.GetComponent<Crosshair> ();

		playerConfig = GameConfig.Instance.GetPlayerConfig(gameObject.tag);

		GameObject opponent = GameObject.Find (GetOpponentName ());
		Vector3 opponentDirection = (opponent.transform.position - transform.position).normalized;
		crosshair.transform.position = opponentDirection * crosshairScript.gamepadDistance;
	}

	void InstantiateFists()
	{
		leftFist = Instantiate (fistPrefab, transform);
		leftFistScript = leftFist.GetComponent<Fist> ();
		leftFistScript.Initialize(Fist.Type.Left, this.tag, damage, forwardFistDistance ,lateralFistDistance);

		rightFist = Instantiate (fistPrefab, transform);
		rightFistScript = rightFist.GetComponent<Fist> ();
		rightFistScript.Initialize(Fist.Type.Right, this.tag, damage, forwardFistDistance ,lateralFistDistance);
	}

	void UpdateBatteryPosition()
	{
		Vector3 batteryPosition = (target - transform.position).normalized * batteryDistance;
		batteryPosition.y = 0.0f;
		battery.transform.localPosition = batteryPosition;
	}

	void UpdateFistsPosition()
	{
		leftFistScript.SetTarget (target);
		rightFistScript.SetTarget (target);

		updateTarget = false;
	}

    void UpdateAnimation()
    {
        Vector3 lookDirection = (transform.position - target).normalized;
        bool isBackAnimator = animator.GetBool("IsBack");

        if ((lookDirection.x > 0.1) && !spriteRenderer.flipX)
            spriteRenderer.flipX = true;
        else if ((lookDirection.x < 0.1) && spriteRenderer.flipX)
            spriteRenderer.flipX = false;

        if ((lookDirection.z < 0.1) && !isBackAnimator)
            animator.SetBool("IsBack", true);
        else if ((lookDirection.z > 0.1) && isBackAnimator)
            animator.SetBool("IsBack", false);
    }

    void OnShootButtonPressed()
	{
		if (playerHealthScript.IsKnockedOut ()) {
			return;
		}
		if (batteryManager.HasBattery ()) {
			batteryManager.DropBattery ();
			return;
		}

		ShootFist ();
	}

	void ShootFist()
	{
		if (leftFistScript.GetState () == Fist.State.Idle) {
			leftFistScript.Shoot ();
		} else if (rightFistScript.GetState () == Fist.State.Idle) {
			rightFistScript.Shoot ();
		}
	}

	void ReadInput()
	{
		ReadTargetInput ();
		ReadShootInput ();
	}

	void ReadTargetInput()
	{
		updateTarget = true;
		target = crosshairScript.worldPosition;
	}

	void ReadShootInput()
	{
		if (this.playerConfig.unityControllerType == ControllerType.KeyboardMouse) {
			isShootButtonPressed = Input.GetMouseButtonDown (0);
		} else if (this.playerConfig.unityControllerType == ControllerType.xBox360Windows) {
			isShootButtonPressed = Input.GetKeyDown (Controllers.xBox360.Windows.ShootButton (this.playerConfig.playerNumber));
		} else if (this.playerConfig.unityControllerType == ControllerType.xBox360Mac) {
			isShootButtonPressed = Input.GetKeyDown (Controllers.xBox360.Mac.ShootButton (this.playerConfig.playerNumber));
		} else if (this.playerConfig.unityControllerType == ControllerType.xBox360Linux) {
			isShootButtonPressed = Input.GetKeyDown (Controllers.xBox360.Linux.ShootButton (this.playerConfig.playerNumber));
		}
	}

	string GetOpponentName()
	{
		if (this.tag == "Red") {
			return "BluePlayer";
		} else {
			return "RedPlayer";
		}
	}

	private GameManager gameManager;
	private PlayerBatteryManager batteryManager;

	private GameConfig.PlayerConfig playerConfig;
	private GameObject battery;
	private GameObject leftFist;
	private Fist leftFistScript;
	private GameObject rightFist;
	private Fist rightFistScript;
	private GameObject crosshair;
	private Crosshair crosshairScript;
	private PlayerHealth playerHealthScript;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isInputEnabled = true;
	private Vector3 target;
	private bool updateTarget = false;
	private bool isShootButtonPressed = false;
}