using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public Vector3 worldPosition;
    [Range(1f, 20f)]
    public float mouseSensivity = 10;
    [Range(1f, 20f)]
    public float gamepadDistance = 100f;

    private GameConfig.PlayerConfig playerConfig;
    private GameManager gameManager;
    private GameObject playerObject;
    private Vector3 gamepadDirection;
    private Rect screenRect;
    private Plane groundPlane;

    void Awake()
    {
        gameManager = GameObject.Find("Level").GetComponent<GameManager>();
        screenRect = Camera.main.gameObject.transform.Find("UI").GetComponent<RectTransform>().rect;
        groundPlane = new Plane(new Vector3(0f, 0.5f, 0f), Vector3.up);
    }

    void Start()
    {
        playerConfig = GameConfig.Instance.GetPlayerConfig(this.tag);
        playerObject = GameObject.Find(this.tag + "Player");

		transform.localPosition = new Vector3 (0.0f, 0.0f, -1.0f) * gamepadDistance;
    }

    void FixedUpdate()
    {
        if (gameManager.gameState != GameState.InGame)
            return;

		if (this.playerConfig.unityControllerType == ControllerType.KeyboardMouse)
        {
//            Vector3 mouseOffset = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f) * this.mouseSensivity;
//            transform.position += mouseOffset;
			transform.position = Input.mousePosition;
        }
        else
        {
            Vector3 newGamepadDirection = Vector3.zero;

			if (this.playerConfig.unityControllerType == ControllerType.xBox360Windows)
            {
				newGamepadDirection.x = Input.GetAxis(Controllers.xBox360.Windows.AimAxisX(this.playerConfig.playerNumber));
				newGamepadDirection.y = -Input.GetAxis(Controllers.xBox360.Windows.AimAxisY(this.playerConfig.playerNumber));
            }
			else if (this.playerConfig.unityControllerType == ControllerType.xBox360Mac)
            {
				newGamepadDirection.x = Input.GetAxis(Controllers.xBox360.Mac.AimAxisX(this.playerConfig.playerNumber));
				newGamepadDirection.y = -Input.GetAxis(Controllers.xBox360.Mac.AimAxisY(this.playerConfig.playerNumber));
            }
			else if (this.playerConfig.unityControllerType == ControllerType.xBox360Linux)
            {
				newGamepadDirection.x = Input.GetAxis(Controllers.xBox360.Linux.AimAxisX(this.playerConfig.playerNumber));
				newGamepadDirection.y = -Input.GetAxis(Controllers.xBox360.Linux.AimAxisY(this.playerConfig.playerNumber));
            }

			if (newGamepadDirection.magnitude > 0.5) {
				gamepadDirection = newGamepadDirection.normalized;
			} else {
				gamepadDirection = new Vector3(0.0f, 0.0f, -1.0f);
			}

			Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint (playerObject.transform.position);

			transform.position = playerScreenPosition + (gamepadDirection * gamepadDistance);
        }

        //Clamp to screen limits
        Vector3 clampedPosition = this.transform.localPosition;
        clampedPosition.x = Mathf.Clamp(this.transform.localPosition.x, screenRect.min.x, screenRect.max.x);
        clampedPosition.y = Mathf.Clamp(this.transform.localPosition.y, screenRect.min.y, screenRect.max.y);
        transform.localPosition = clampedPosition;

        //Translate to world position
        float rayDistance;
        Ray ray = Camera.main.ScreenPointToRay(transform.position);
        Debug.DrawRay(ray.origin, ray.direction, Color.cyan);
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            worldPosition = ray.GetPoint(rayDistance);
            worldPosition.y = 0.5f;
        }
            
    }
}