using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    private float xMovement;
    private float zMovement;

    private float whaleSpeed = 0f;
    private float turnSpeed = 15f;
    private int canJump = 0;
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
        //lock y rotation to 0 so the whale can't be flipped over (for now)
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);

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
        Dive();
        if (canJump >= 100)
        {
            canJump = 100;
        }
        else
        {
            canJump++;
        }
        Jump();


    }

    void Turn()
    {
        if (Input.GetAxisRaw("Horizontal") <0)
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);

        if (Input.GetAxisRaw("Horizontal") > 0 )
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
    }
    void Jump()
    {
        if (canJump == 100)
        {
            if (Input.GetButton("Jump") && HeightInWater.underwater == false)
            {
                whaleBody.AddRelativeForce(0, 100, -30, ForceMode.Impulse);
                canJump = 0;
            }
        }
    }
    void Dive()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            HeightInWater.underwater = !HeightInWater.underwater;
        }
    }
}
