using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    private float xMovement;
    private float zMovement;
    public Animator whaleAnimator;
    private AnimationHashTable animations;

    private float whaleSpeed = 0f;
    public float turnSpeed = 15f;
    private int canJump = 0;
    public float accel = .5f;

    private bool diving = false;

    private Rigidbody whaleBody;
    private void Awake()
    {
        whaleBody = GetComponent<Rigidbody>();
        whaleAnimator = gameObject.GetComponent<Animator>();
    }
    // Update is called once per frame
    void FixedUpdate () {
        if (animations == null)
        {
            animations = GameManager.gmInst.GetComponent<AnimationHashTable>();
        }
        //cheat code - press N+M+RSHIFT to add a lap to your lap counter if you're testing the endgame
        if ((Input.GetKey(KeyCode.M) && Input.GetKey(KeyCode.N)) && Input.GetKeyDown(KeyCode.RightShift))
        {
            gameObject.GetComponent<PlayerManager>().pmInstance.newLap();
        }

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
            whaleAnimator.SetFloat(animations.moveFloat, whaleSpeed / 2);

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
            whaleAnimator.SetFloat(animations.moveFloat, whaleSpeed / 2);

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
            whaleAnimator.SetFloat(animations.moveFloat, whaleSpeed / 2);

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
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
            //whaleAnimator.SetFloat(animations.turnFloat, turnSpeed);
        }

        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
           //whaleAnimator.SetFloat(animations.turnFloat, turnSpeed);
        }

    }
    void Jump()
    {
        if (canJump == 100)
        {
            if (Input.GetButton("Jump") && HeightInWater.underwater == false)
            {
                whaleAnimator.SetBool(animations.jumpBool, true);
                whaleBody.AddRelativeForce(0, 100, -30, ForceMode.Impulse);
                canJump = 0;
            }
        }
        else
        {
            whaleAnimator.SetBool(animations.jumpBool, true);

        }
    }
    void Dive()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            HeightInWater.underwater = !HeightInWater.underwater;
            if (HeightInWater.underwater)
            {
                whaleAnimator.SetBool(animations.underwaterBool, false);

                whaleAnimator.SetBool(animations.diveBool, true);
                whaleAnimator.SetBool(animations.subMovementBool, true);
                whaleAnimator.speed = .5f;

            }
            else
            {
                whaleAnimator.SetBool(animations.underwaterBool, true);
                whaleAnimator.SetBool(animations.diveBool, false);
                whaleAnimator.SetBool(animations.subMovementBool, false);
                whaleAnimator.speed = 1f;


            }
        }
        else
        {

            whaleAnimator.SetBool(animations.diveBool, false);
        }
    }
}
