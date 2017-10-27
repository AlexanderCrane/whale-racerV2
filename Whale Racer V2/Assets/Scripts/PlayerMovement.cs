using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    private float xMovement;
    private float zMovement;

    private float whaleSpeed = 90f;
    private float turnSpeed = 80f;

    private Rigidbody whaleBody;
    private void Awake()
    {
        whaleBody = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void FixedUpdate () {
        xMovement = Input.GetAxis("Horizontal");
        zMovement = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(0.0f, 0.0f, zMovement);
        Debug.Log(xMovement);
        Turn();
        whaleBody.AddRelativeForce(movement * -whaleSpeed);
	}

    void Turn()
    {
        if (Input.GetAxisRaw("Horizontal") <0)
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);

        if (Input.GetAxisRaw("Horizontal") > 0 )
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);

    }
}
