using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class PlayerShoot : MonoBehaviour {
	public GameObject crosshairPrefab;
	public GameObject bulletPrefab;
	public float bulletSpeed = 10.0f;
	public int bulletDamage = 5;
	public AudioClip shotSound;

	private GameObject crosshair;
	private Crosshair crosshairScript;
	private PlayerHealth playerHealthScript;
	private PlayerBatteryManager playerBatteryManager;
    private GameConfig.PlayerConfig playerConfig;
    private GameManager gameManager;
    private LineRenderer lineRenderer;
    private int layerMask;
    private Color laserColor;
	private AudioSource audioSource;

    private void Awake()
    {
        gameManager = GameObject.Find("Level").GetComponent<GameManager>();
        lineRenderer = transform.GetComponent<LineRenderer>();
        layerMask = LayerMask.GetMask(new string[] {"Wall" , "Stage"});
        
        
    }

    void Start () {
		GameObject ui = GameObject.Find ("UI");

		crosshair = Instantiate (crosshairPrefab, ui.transform);
		crosshair.name = gameObject.tag + "Crosshair";
        crosshair.tag = gameObject.tag;

        crosshairScript = crosshair.GetComponent<Crosshair> ();
	
		playerConfig = GameConfig.Instance.GetPlayerConfig(gameObject.tag);
		Image crosshairImage = crosshair.GetComponent<Image> ();
		crosshairImage.color = playerConfig.crosshairColor;
        laserColor = playerConfig.crosshairColor;
        laserColor.a = 1f;
        lineRenderer.startColor = lineRenderer.endColor = laserColor;

        playerHealthScript = gameObject.GetComponent<PlayerHealth> ();
		playerBatteryManager = gameObject.GetComponent<PlayerBatteryManager> ();
		audioSource = GetComponent<AudioSource>();
    }

	void Update () {
        if (gameManager.gameState != GameState.InGame)
            return;

        UpdateLaser();

        bool shoot = false;

        if (this.playerConfig.controllerType == ControllerType.KeyboardMouse)
        {
            shoot = Input.GetMouseButtonDown(0);
        }
        else if (this.playerConfig.controllerType == ControllerType.xBox360Windows)
        {
            shoot = Input.GetKeyDown(Controllers.xBox360.Windows.ShootButton(this.playerConfig.playerNumber));
        }
        else if (this.playerConfig.controllerType == ControllerType.xBox360Mac)
        {
            shoot = Input.GetKeyDown(Controllers.xBox360.Mac.ShootButton(this.playerConfig.playerNumber));
        }
        else if (this.playerConfig.controllerType == ControllerType.xBox360Linux)
        {
            shoot = Input.GetKeyDown(Controllers.xBox360.Linux.ShootButton(this.playerConfig.playerNumber));
        }

        if (shoot)
            Shoot(crosshairScript.worldPosition);
    }

    void UpdateLaser()
    {
        RaycastHit hit;
        Vector3 startPosition = new Vector3(transform.position.x, 0.5f, transform.position.z);
        Vector3 laserDirection = crosshairScript.worldPosition - startPosition;
        Physics.Raycast(startPosition, laserDirection, out hit, 1000f, layerMask);

        //Set beam line
        Vector3[] beamPoints = new Vector3[2];
        beamPoints[0] = this.transform.position;
        beamPoints[1] = hit.point;

        //Draw beam line
        for (int i = 0; i < beamPoints.Length; i++)
            lineRenderer.SetPosition(i, beamPoints[i]);
    }

	void Shoot(Vector3 targetPosition)
	{
		if (playerHealthScript.IsKnockedOut ()) {
			return;
		}
		if (playerBatteryManager.HasBattery ()) {
			playerBatteryManager.DropBattery ();
			return;
		}

		GameObject bullet = Instantiate (bulletPrefab, transform.position, Quaternion.identity);

		Bullet bulletScript = bullet.GetComponent<Bullet> ();

		Vector3 playerPosition = transform.position;
	
		// Cancel y coordinate
		playerPosition.y = 0.0f;
		targetPosition.y = 0.0f;

		Vector3 shootDirection = targetPosition - playerPosition;

		bulletScript.Shoot (bulletSpeed * shootDirection.normalized, gameObject.tag, bulletDamage);
		audioSource.PlayOneShot (shotSound);
	}
}
