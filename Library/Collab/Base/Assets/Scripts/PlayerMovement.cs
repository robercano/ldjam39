using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float movementSpeed = 5f;

    private Rigidbody refRigidbody;

	void Awake () {
        this.refRigidbody = this.GetComponent<Rigidbody>();
    }
	
	void Update () {

        Vector3 movementDirection = Vector3.zero;
        movementDirection.x = this.IsMoveRight() ? 1f : (this.IsMoveLeft() ? -1f : 0f);
        movementDirection.z = this.IsMoveUp() ? 2f : (this.IsMoveDown() ? -2f : 0f);

        this.refRigidbody.velocity = movementDirection.normalized * this.movementSpeed;
    }

    public bool IsMoveRight()
    {
        if (Input.GetKey(KeyCode.D))
            return true;

        return false;
    }

    public bool IsMoveLeft()
    {
        if (Input.GetKey(KeyCode.A))
            return true;

        return false;
    }

    public bool IsMoveUp()
    {
        if (Input.GetKey(KeyCode.W))
            return true;

        return false;
    }

    public bool IsMoveDown()
    {
        if (Input.GetKey(KeyCode.S))
            return true;

        return false;
    }
}
