using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : MonoBehaviour {

	public float speed = 1.0f;
	public float homingAngle = 1.0f;
	public AudioClip shotSound;
	public AudioClip hitWallSound;
	public AudioClip hitRobotSound;

	public enum State
	{
		Idle,
		Shot
	}
	public enum Type
	{
		Left,
		Right
	}

	public State GetState()
	{
		return state;
	}

	public void Initialize(Type type, string tag, int damage, float lateralDistance, float forwardDistance, PlayerFistAttack playerFistAttack)
	{
		SetType (type);
		SetTag (tag);
		SetDamage (damage);
		SetLateralDistance (lateralDistance);
		SetForwardDistance (forwardDistance);

		layerMask = LayerMask.GetMask(new string[] {"Player", "Stage", "Wall"});

		opponent = GameObject.Find (GetOpponentName ());
        this.playerFistAttack = playerFistAttack;

    }

	public void SetType(Type type)
	{
		this.type = type;
	}

	public void SetTag(string tag)
	{
		this.tag = tag;
	}

	public void SetDamage(int damage)
	{
		this.damage = damage;
	}

	public void SetLateralDistance(float lateralDistance)
	{
		this.lateralDistance = lateralDistance;
	}

	public void SetForwardDistance(float forwardDistance)
	{
		this.forwardDistance = forwardDistance;
	}

	public void SetTarget(Vector3 target)
	{
		if (state == State.Shot) {
			return;
		}

		Vector3  direction = target - transform.position;
		if (direction.magnitude < 1.0f) {
			return;
		}

		forwardDirection = direction.normalized;
		forwardDirection.y = 0.0f;
	}

	public void Shoot()
	{
		SetState (State.Shot);
	}

	void Awake()
	{
		playerLayerID = LayerMask.NameToLayer ("Player");

		rigidBody = GetComponent<Rigidbody> ();
		sphereCollider = GetComponent<SphereCollider> ();
        spriteRenderer = this.transform.Find("Sprite").GetComponent<SpriteRenderer>();

        sphereCollider.enabled = false;
		this.name = "Fist";
	}

	void Start()
	{
        cameraManager = Camera.main.gameObject.GetComponent<CameraManager>();
        player = transform.parent;
	}

	void FixedUpdate () {
		UpdateFistsPosition ();
	}

	void UpdateFistsPosition ()
	{
		switch (state) {
		case State.Idle:
			UpdateIdleFist ();
			break;
		case State.Shot:
			UpdateShotFist ();
			break;
		}

    }

	void UpdateIdleFist ()
	{
		Vector3 forwardDirection = GetForwardVector ();
		Vector3 rightDirection = GetRightDirection ();
	
		Vector3 fistPosition = forwardDirection * forwardDistance;
		if (type == Type.Left) {
			fistPosition -= rightDirection * lateralDistance;
		} else {
			fistPosition += rightDirection * lateralDistance;
		}

		fistPosition.y = 0.0f;
		transform.localPosition = fistPosition;

        spriteRenderer.sortingOrder = (int)playerFistAttack.transform.position.z * -100;
    }
		
	void UpdateShotFist()
	{
		Vector3 opponentDirection = (opponent.transform.position - rigidBody.transform.position).normalized;

		RaycastHit hit;
		bool hasHit = Physics.Raycast (transform.position, opponentDirection, out hit, 1000f, layerMask);

		if (hasHit == false) {
			return;
		}
			
		if (hit.collider.gameObject.tag != GetOpponentGameTag ()) {
			return;
		}

		rigidBody.velocity = Vector3.RotateTowards(rigidBody.velocity, opponentDirection, homingAngle * Mathf.Deg2Rad, 0.0f);
		lastRigidBodyVelocity = rigidBody.velocity;

        spriteRenderer.sortingOrder = (int)this.transform.position.z * -100;
    }

	public Vector3 GetDirectionFromAngle(float angle)
	{
		return new Vector3(Mathf.Cos(angle*Mathf.Deg2Rad), 0.0f, Mathf.Sin(angle*Mathf.Deg2Rad));
	}

	public float GetAngleFromDirection(Vector3 direction)
	{
		return NormalizeAngle(Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg);
	}

	public float NormalizeAngle(float angle)
	{
		angle = angle % 360;

		if (angle < 0)
			angle += 360;

		return angle;
	}

	Vector3 GetForwardVector()
	{
		return forwardDirection;
	}

	Vector3 GetRightDirection()
	{
		Vector3 forwardDirection = GetForwardVector ();
		Vector3 rightDirection = Vector3.Cross (forwardDirection, Vector3.up);

		return rightDirection;
	}

	Vector2 GetLeftDirection()
	{
		return -GetRightDirection ();
	}

	void SetState(State newState)
	{
		if (state == newState) {
			return;
		}

		state = newState;

		// Execute onEnter for the state here 
		switch (state) {
		case State.Idle:
			OnEnterIdleState ();
			break;
		case State.Shot:
			OnEnterShootState ();
			break;
		}
	}

	void OnEnterIdleState()
	{
		sphereCollider.enabled = false;
		transform.parent = player;
		rigidBody.velocity = Vector3.zero;
	}

	void OnEnterShootState()
	{
		sphereCollider.enabled = true;
		transform.parent = null;

		AudioSource.PlayClipAtPoint (shotSound, Camera.main.transform.position);

		rigidBody.velocity = forwardDirection * speed;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == this.tag) {
			// Collider is ourselves
			return;
		}

		if (col.gameObject.layer == playerLayerID) {
            cameraManager.AddCameraShake(0.4f, lastRigidBodyVelocity.normalized);
            OnCollisionWithOpponent (col.gameObject);
		} else {
            cameraManager.AddCameraShake(0.2f, lastRigidBodyVelocity.normalized);
            AudioSource.PlayClipAtPoint (hitWallSound, Camera.main.transform.position);
		}

		SetState (State.Idle);
	}

	void OnCollisionWithOpponent(GameObject player)
	{
		// TODO: player.GetComponent() can be moved to GameManager so the component is already fetched
		PlayerHealth playerHealthScript = player.GetComponent ("PlayerHealth") as PlayerHealth;
		playerHealthScript.ApplyDamage (damage, lastRigidBodyVelocity);
		AudioSource.PlayClipAtPoint (hitRobotSound, Camera.main.transform.position);
	}
	
	string GetOpponentGameTag()
	{
		if (this.tag == "Red") {
			return "Blue";
		} else {
			return "Red";
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

	State state = State.Idle;
	Type type;
	float lateralDistance;
	float forwardDistance;
	Transform player;
	Vector3 forwardDirection = Vector3.right;
	Rigidbody rigidBody;
	SphereCollider sphereCollider;
    SpriteRenderer spriteRenderer;
    PlayerFistAttack playerFistAttack;
    int damage;
	int playerLayerID;
	Vector3 lastRigidBodyVelocity = Vector3.up;
    CameraManager cameraManager;
    GameObject opponent;
	int layerMask;
}
