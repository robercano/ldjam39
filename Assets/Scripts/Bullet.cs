using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	public float maxLifeTime = 10.0f;

	private float spawnTime;

	private int damage;
	private Rigidbody rigidBody;
	private int playerLayerID;
	private int stageLayerID;

	#region - Public methods
	public void Shoot(Vector3 velocity, string tag, int damage)
	{
		rigidBody.velocity = velocity;
		gameObject.tag = tag;
		this.damage = damage;

		spawnTime = Time.time; 
	}
	#endregion // Public methods

	#region - Private methods
	void Awake () {
		rigidBody = GetComponent<Rigidbody> ();	

		playerLayerID = LayerMask.NameToLayer ("Player");
		stageLayerID = LayerMask.NameToLayer ("Stage");
	}

	void Update()
	{
		if (Time.time - spawnTime > maxLifeTime) {
			GameObject.DestroyObject (gameObject);
		}
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.layer == playerLayerID) {
			HandleCollisionWithPlayer (other.gameObject);
		} else if (other.gameObject.layer == stageLayerID) {
			Destroy (gameObject);
		}
	}

	void HandleCollisionWithPlayer(GameObject player)
	{
		if(player.tag == gameObject.tag) {
			// Same team, bail out
			return;
		}

		// TODO: player.GetComponent() can be moved to GameManager so the component is already fetched
		PlayerHealth playerHealthScript = player.GetComponent ("PlayerHealth") as PlayerHealth;
		playerHealthScript.ApplyDamage (damage, rigidBody.velocity);
		Destroy (gameObject);
	}
	#endregion // Private methods
}
