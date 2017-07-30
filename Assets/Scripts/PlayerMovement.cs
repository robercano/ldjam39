﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState {
    Idle = 0,
    Run = 1,
    Stun = 2
}

public class PlayerMovement : MonoBehaviour {

    public float movementSpeed = 5f;

    private GameConfig.PlayerConfig playerConfig;
    private Rigidbody refRigidbody;
    private Animator refAnimator;
    private float axisMargin = 0.2f;
	private GameManager gameManager;


	private PlayerHealth playerHealthScript;
	private Vector3 lastMovementDirection = Vector3.zero;

    void Awake () {
		gameManager = GameObject.Find("Level").GetComponent<GameManager>();
        this.refRigidbody = this.GetComponent<Rigidbody>();
        this.refAnimator = this.GetComponent<Animator>();
    }

	void Start () {
		playerHealthScript = gameObject.GetComponent<PlayerHealth> ();
        playerConfig = GameConfig.Instance.GetPlayerConfig(this.tag);
        this.refAnimator.SetBool("IsRed", (this.tag == "Red"));
    }
	
	void Update () {
		if (gameManager.gameState != GameState.InGame)
			return;

		if (playerHealthScript.IsKnockedOut ()) {
			return;
		}
        Vector3 movementDirection = Vector3.zero;
        movementDirection.x = this.IsMoveRight() ? 1f : (this.IsMoveLeft() ? -1f : 0f);
        movementDirection.z = this.IsMoveUp() ? 2f : (this.IsMoveDown() ? -2f : 0f);

		if (movementDirection != Vector3.zero) {
			lastMovementDirection = movementDirection;
		}

        this.refRigidbody.velocity = movementDirection.normalized * this.movementSpeed;

        //Idle and Run animations
        if (this.refRigidbody.velocity.magnitude > 0f)
            this.refAnimator.SetInteger("State", (int)PlayerState.Run);
        else if (this.refAnimator.GetInteger("State") != (int)PlayerState.Stun)
            this.refAnimator.SetInteger("State", (int)PlayerState.Idle);
    }

	public void StopPlayer() {
		this.refRigidbody.velocity = Vector3.zero;	
	}

    public bool IsMoveRight()
    {
        if (this.playerConfig.controllerType == ControllerType.KeyboardMouse)
        {
            return (Input.GetKey(KeyCode.D));
        }
        else if (this.playerConfig.controllerType == ControllerType.xBox360Windows)
        {
            return (Input.GetAxis(Controllers.xBox360.Windows.MoveAxisX(this.playerConfig.playerNumber)) > this.axisMargin);
        }
        else if (this.playerConfig.controllerType == ControllerType.xBox360Mac)
        {
            return (Input.GetAxis(Controllers.xBox360.Mac.MoveAxisX(this.playerConfig.playerNumber)) > this.axisMargin);
        }
        else if (this.playerConfig.controllerType == ControllerType.xBox360Linux)
        {
            return (Input.GetAxis(Controllers.xBox360.Linux.MoveAxisX(this.playerConfig.playerNumber)) > this.axisMargin);
        }

        return false;
    }

    public bool IsMoveLeft()
    {
        if (this.playerConfig.controllerType == ControllerType.KeyboardMouse)
        {
            return (Input.GetKey(KeyCode.A));
        }
        else if (this.playerConfig.controllerType == ControllerType.xBox360Windows)
        {
            return (Input.GetAxis(Controllers.xBox360.Windows.MoveAxisX(this.playerConfig.playerNumber)) < -this.axisMargin);
        }
        else if (this.playerConfig.controllerType == ControllerType.xBox360Mac)
        {
            return (Input.GetAxis(Controllers.xBox360.Mac.MoveAxisX(this.playerConfig.playerNumber)) < -this.axisMargin);
        }
        else if (this.playerConfig.controllerType == ControllerType.xBox360Linux)
        {
            return (Input.GetAxis(Controllers.xBox360.Linux.MoveAxisX(this.playerConfig.playerNumber)) < -this.axisMargin);
        }

        return false;
    }

    public bool IsMoveUp()
    {
        if (this.playerConfig.controllerType == ControllerType.KeyboardMouse)
        {
            return (Input.GetKey(KeyCode.W));
        }
        else if (this.playerConfig.controllerType == ControllerType.xBox360Windows)
        {
            return (Input.GetAxis(Controllers.xBox360.Windows.MoveAxisY(this.playerConfig.playerNumber)) < -this.axisMargin);
        }
        else if (this.playerConfig.controllerType == ControllerType.xBox360Mac)
        {
            return (Input.GetAxis(Controllers.xBox360.Mac.MoveAxisY(this.playerConfig.playerNumber)) < -this.axisMargin);
        }
        else if (this.playerConfig.controllerType == ControllerType.xBox360Linux)
        {
            return (Input.GetAxis(Controllers.xBox360.Linux.MoveAxisY(this.playerConfig.playerNumber)) < -this.axisMargin);
        }

        return false;
    }

    public bool IsMoveDown()
    {
        if (this.playerConfig.controllerType == ControllerType.KeyboardMouse)
        {
            return (Input.GetKey(KeyCode.S));
        }
        else if (this.playerConfig.controllerType == ControllerType.xBox360Windows)
        {
            return (Input.GetAxis(Controllers.xBox360.Windows.MoveAxisY(this.playerConfig.playerNumber)) > this.axisMargin);
        }
        else if (this.playerConfig.controllerType == ControllerType.xBox360Mac)
        {
            return (Input.GetAxis(Controllers.xBox360.Mac.MoveAxisY(this.playerConfig.playerNumber)) > this.axisMargin);
        }
        else if (this.playerConfig.controllerType == ControllerType.xBox360Linux)
        {
            return (Input.GetAxis(Controllers.xBox360.Linux.MoveAxisY(this.playerConfig.playerNumber)) > this.axisMargin);
        }

        return false;
    }

	public Vector3 GetPositionToDropFromDirection(Vector3 direction) {
		return gameObject.transform.position + (1.5f * direction.normalized);
	}

	public Vector3 GetPositionInFront() {
		Vector3 frontDirection = GetFrontDirection();
		return GetPositionToDropFromDirection(frontDirection);
	}

	public Vector3 GetPositionInBack() {
		Vector3 backDirection = GetBackDirection();
		return GetPositionToDropFromDirection(backDirection);
	}

	public Vector3 GetFrontDirection() {
		Vector3 frontDirection = lastMovementDirection.normalized;
		if (lastMovementDirection == Vector3.zero) {
			frontDirection =  new Vector3 (1.0f, 0.0f, 0.0f);
		}
		return frontDirection;
	}

	public Vector3 GetBackDirection() {
		return -1.0f * GetFrontDirection ();
	}

	public Vector3 GetPlayerSpeed() {
		return this.refRigidbody.velocity;
	}
}