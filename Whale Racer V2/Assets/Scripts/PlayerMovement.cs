using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    private float xMovement;
    private float zMovement;

    private float whaleSpeed = 0f;
    private float turnSpeed = 10f;
    public float accel = .5f;

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
        //going forward
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            if (whaleSpeed < 17f){
                whaleSpeed += accel;
            }
            else
            {
                whaleSpeed = 18f;
            }
        }
        //backing up
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            if (whaleSpeed < 6f)
            {
                whaleSpeed += accel;
         
            }
            else
            {
                whaleSpeed = 7f;
            }
        }
        //stopping
        else
        {
            if (whaleSpeed < -1)
            {
                whaleSpeed += accel/5;
            }
            else
            {
                whaleSpeed = 0.0f;
            }
        }
        whaleBody.AddRelativeForce(0.0f, 0.0f, zMovement * (-whaleSpeed));
        Turn();

    }

    void Turn()
    {
        if (Input.GetAxisRaw("Horizontal") <0)
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);

        if (Input.GetAxisRaw("Horizontal") > 0 )
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);

    }
}
