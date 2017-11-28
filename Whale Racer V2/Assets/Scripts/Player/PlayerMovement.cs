using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine;
/// <summary>
///Player movement control script.
/// </summary>
public class PlayerMovement : NetworkBehaviour
{
    #region public variables and properties
    public Animator whaleAnimator;
    public AnimationHashTable animations;

    public float whaleSpeed = 0f;
    public float turnSpeed = 40.0f;
    public float baseTurnSpeed = 500.0f;
    public float accel = 100.0f;
    public float maxForwardSpeed = 1650.0f;
    public float maxBackwardSpeed = 700.0f;
    public float baseMaxForward = 800.0f;
    public float baseMaxBackward = 100.0f;
    public float sprintMultiplier = 1.8f;
    public float underwaterMod = 1.0f; //it is 2.0f when underwater
    public Rigidbody whaleBody;

    public bool splitscreenPlayer2 = false;
    public bool freeFormMovement = false;

    public PlayerHealth playerHealth;
    #endregion

    #region private variables and properties
    [SerializeField] private WhaleDirect wDirect;
    [SerializeField] private Camera playerCam;

    private float xMovement;
    private float zMovement;
    private float speedupDuration = 0;
    private float speedupStart = 0;
    private float xRotation = 0;
    private float yRotation = 0;
    private float zRotation = 0;
    private int canJump = 0;
    private int canSprint = 0;
    private bool diving = false;
    private string jumpButton = "Jump";
    private string diveButton = "Dive";
    private string sprintButton = "Sprint";
    private bool spedup = false;
    private bool underWater = true;
    private bool movementAudioPlaying = false;
    private string horizontalAxis = "Horizontal";
    private string verticalAxis = "Vertical";
    private string spiralAxis = "Spiral";
    private bool noTurn = false;
    private bool noSpiral = false;
    #endregion
    /// <summary>
    /// Awake method. Stores base values for forward/backward/turn speed. Initializes animator and rigidbody.
    /// </summary>
    private void Awake()
    {
        whaleBody = this.GetComponent<Rigidbody>();
        whaleAnimator = gameObject.GetComponent<Animator>();
        baseMaxForward = maxForwardSpeed;
        baseMaxBackward = maxBackwardSpeed;
        baseTurnSpeed = turnSpeed;
        Debug.Log("CAMS STARTING");

        if (splitscreenPlayer2)
        {
            Debug.Log("Setting axes to p2 axes.");
            horizontalAxis = "p2Horizontal";
            verticalAxis = "p2Vertical";
            spiralAxis = "p2Spiral";
            jumpButton = "p2Jump";
            diveButton = "p2Dive";
            sprintButton = "p2Sprint";
        }

        //health
        playerHealth = this.GetComponent<PlayerHealth>();
        playerHealth.currentHealth = 100;
        Debug.Log("Health worked");
        if (freeFormMovement)
        {
            whaleBody.constraints = RigidbodyConstraints.None;
        }
        else
        {
            whaleBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |RigidbodyConstraints.FreezeRotationZ;
        }
    }
    /// <summary>
    /// Update method. Calls methods to move and rotate player
    /// </summary>
    private void Update()
    {

    }
    /// <summary>
    /// Fixed Update method. Calls methods to player input to whale movement.
    /// Checks for power up expiration and handles jump lockout.
    /// </summary>
    void FixedUpdate()
    {

        if (playerCam == null)
        {
            Debug.Log("CAM: " + gameObject.GetComponent<PlayerManager>().cam.gameObject.name);
            playerCam = gameObject.GetComponent<PlayerManager>().cam.gameObject.GetComponent<Camera>();
            if (wDirect.initialized == false)
            {
                wDirect.Init(transform, playerCam.transform);
            }
        }
        if (GameManager.gmInst.isMP && !isLocalPlayer)
        {
            return;
        }
        if (animations == null)
        {
            animations = GameManager.gmInst.GetComponent<AnimationHashTable>();
        }
        if (freeFormMovement)
        {
            RotatePlayer();
        }

        if (noTurn && noSpiral)
        {
            PositionReset(0); //reset x and z
        }
        else if (transform.position.y > -2)
        {
            float xAngle = this.transform.rotation.eulerAngles.x;
            if (xAngle > 40.0f || xAngle < -70.0f)
            {
                PositionReset(0);
            }
            NoFlyingWhales();
        }

        if (transform.position.y > -2.0f) { underwaterMod = 1.0f; }
        else { underwaterMod = 2.0f; }

        float spiralInput = Input.GetAxis(spiralAxis);
        xMovement = Input.GetAxis(horizontalAxis);
        zMovement = Input.GetAxis(verticalAxis);
        Move2D(zMovement);
        noTurn = Turn(zMovement, xMovement);
        noSpiral = Spiral(spiralInput);

        Dive(Input.GetButton(diveButton));
        if (canJump >= 100)
        {
            canJump = 100;
        }
        else
        {
            canJump++;
        }
        if (canSprint >= 1000)
        {
            canSprint = 1000;
        }
        else
        {
            canSprint++;
        }
        Jump(Input.GetButton(jumpButton));

        if (GameManager.gmInst.isMP && !isLocalPlayer)
        {
            return;
        }
        if (animations == null)
        {
            animations = GameManager.gmInst.GetComponent<AnimationHashTable>();
        }

        //cheat code - press N+M+RSHIFT to add a lap to your lap counter if you're testing the endgame
        if ((Input.GetKey(KeyCode.M) && Input.GetKey(KeyCode.N)) && Input.GetKeyDown(KeyCode.RightShift))
        {
            gameObject.GetComponent<PlayerManager>().pmInstance.NewLap();
        }
        //xMovement = Input.GetAxis(horizontalAxis);
        //zMovement = Input.GetAxis(verticalAxis);
        //lock y rotation to 0 so the whale can't be flipped over (for now)
        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        if (Time.time - speedupStart > speedupDuration && spedup)
        {
            if (whaleSpeed > baseMaxForward)
            {
                whaleSpeed = baseMaxForward;
            }
            maxBackwardSpeed = baseMaxBackward;
            maxForwardSpeed = baseMaxForward;
            turnSpeed = baseTurnSpeed;
            spedup = false;

            Debug.Log("Buff expired");
        }
        //move and turn were here
        //jum and stuff were here

        if (freeFormMovement)
        {
            wDirect.UpdateLock();
        }
    }
    /// <summary>
    /// Translates player input to forward/backward movement. Plays corresponding audio and animation.
    /// </summary>
    public void Move2D(float input)
    {
        Vector3 movement = new Vector3(0.0f, 0.0f, zMovement);
        AudioSource movementSplash = GetComponent<AudioSource>();
        //going forward
        if (input > 0)
        {
            if (!movementAudioPlaying && movementSplash != null)
            {
                movementSplash.Play();
                movementAudioPlaying = true;
            }
            if (whaleSpeed < maxForwardSpeed)
            {
                whaleSpeed += accel;
            }
            else
            {
                whaleSpeed = maxForwardSpeed + 1;
            }
            whaleAnimator.SetFloat(animations.moveFloat, whaleSpeed / 2);

        }
        //backing up
        else if (input < 0)
        {
            if (!movementAudioPlaying && movementSplash != null)
            {
                movementSplash.Play();
                movementAudioPlaying = true;
            }

            if (whaleSpeed < maxBackwardSpeed)
            {
                whaleSpeed += accel;

            }
            else
            {
                whaleSpeed = maxBackwardSpeed + 1;
            }
            whaleAnimator.SetFloat(animations.moveFloat, whaleSpeed / 2);

        }
        //stopping
        else
        {
            if (movementAudioPlaying && movementSplash != null)
            {
                movementSplash.Stop();
                movementAudioPlaying = false;
            }
            if (whaleSpeed < -1)
            {
                whaleSpeed += accel / 5;
            }
            else
            {
                whaleSpeed = 0.0f;
            }
            whaleAnimator.SetFloat(animations.moveFloat, whaleSpeed / 2);

        }

        if(Input.GetButton(sprintButton) && canSprint == 100)
        {
            Debug.Log("Sprinting");
            whaleBody.AddRelativeForce(0.0f, 0.0f, input * (-whaleSpeed) * sprintMultiplier * underwaterMod);
            whaleAnimator.SetFloat(animations.moveFloat, whaleSpeed*4);
            canSprint = 0;
            return;
        }
        whaleBody.AddRelativeForce(0.0f, 0.0f, input * (-whaleSpeed) * underwaterMod);
    }
    /// <summary>
    /// Update method. Calls methods to move and rotate player
    /// </summary>
    private void RotatePlayer()
    {
        wDirect.RotateAndLook(transform, playerCam.transform);
    }
    /// <summary>
    /// Translates input into rotation.
    /// </summary>
    public bool Turn(float inputVertical, float inputHorizontal)
    {
        int turnSpeedMult = 1;
        if (inputVertical < 0)
        {
            turnSpeedMult = -1;
        }

        if (inputHorizontal != 0)
        {
            if (inputHorizontal < 0)
            {
                transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime * turnSpeedMult);
                //whaleAnimator.SetFloat(animations.turnFloat, turnSpeed);
            }
            else if (inputHorizontal > 0)
            {
                transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime * turnSpeedMult);
            }
            yRotation = this.transform.eulerAngles.y;
            return false;
        }
        return true;
    }
    /// <summary>
    /// Rotates player back to resting position
    /// </summary>
    public void PositionReset(int xAndZ) //0 for x&z, 1 for x, 2 for z
    {
        Quaternion fromRotation = transform.rotation;
        Quaternion targetRotation;
        yRotation = this.transform.eulerAngles.y;
        //X & Z
        if (xAndZ == 0)
        {
            targetRotation = Quaternion.Euler(new Vector3(0.0f, yRotation, 0.0f));
        }
        else if (xAndZ == 1)
        {
            //JUST X
            targetRotation = Quaternion.Euler(new Vector3(0.0f, yRotation, zRotation));
        }
        else
        {
            //JUST Z
            targetRotation = Quaternion.Euler(new Vector3(xRotation, yRotation, 0.0f));
        }

        float xAng = transform.localEulerAngles.x;
        float zAng = transform.localEulerAngles.z;
        if ((xAng > 20.0f || xAng < -10.0f) || (zAng > 10.0f || zAng < -10.0f))
        {
            this.transform.localRotation = Quaternion.Slerp(this.transform.rotation, targetRotation,
                5.50f * Time.deltaTime);
        }
        //whaleBody.AddForce(-Vector3.forward * zMovement * whaleSpeed);
        //whaleAnimator.SetFloat(animations.moveFloat, whaleSpeed * 2);

    }
    /// <summary>
    /// Spirals the whale along z-axis
    /// </summary>
    public bool Spiral(float inputSpiral)
    {
        if (inputSpiral != 0)
        {
            if (this.transform.position.y > -2.0f)// && !Input.GetButton(verticalAxis))
            {
                transform.Rotate(transform.TransformDirection(Vector3.up), inputSpiral * whaleSpeed/25 * Time.deltaTime);
                transform.Rotate(Vector3.forward, inputSpiral * whaleSpeed / 20 * Time.deltaTime);

                yRotation = transform.eulerAngles.y;
                zRotation = transform.eulerAngles.z;
                PositionReset(1);
            }
            else
            {
                transform.Rotate(Vector3.forward, inputSpiral * 200 * Time.deltaTime);
            }
            zRotation = transform.eulerAngles.z;
            return false;
        }
        return true;
    }
    public bool CheckCanJump()
    {
        return (canJump == 100);
    }
    public void SetCanJump()
    {
        canJump = 100;
    }
    public bool CheckCanSprint()
    {
        return (canSprint == 100);
    }
    public void SetCanSprint()
    {
        canSprint = 100;
    }
    /// <summary>
    /// Handles jump input.
    /// </summary>
    public void Jump(bool jumpPressed)
    {
        if (CheckCanJump())
        {
            if (jumpPressed && HeightInWater.underwater == false)
            {
                whaleAnimator.SetBool(animations.jumpBool, true);
                whaleBody.AddRelativeForce(0, 400, -30, ForceMode.Impulse);
                canJump = 0;
            }
        }
        else
        {
            whaleAnimator.SetBool(animations.jumpBool, true);
        }
    }
    private void DiveAnimation()
    {
        whaleAnimator.SetBool(animations.underwaterBool, false);
        whaleAnimator.SetBool(animations.diveBool, true);
        whaleAnimator.SetBool(animations.subMovementBool, true);
        whaleAnimator.speed = 0.9f;
    }
    private void RoamAnimation()
    {
        whaleAnimator.SetBool(animations.underwaterBool, true);
        whaleAnimator.SetBool(animations.diveBool, false);
        whaleAnimator.SetBool(animations.subMovementBool, false);
        whaleAnimator.speed = 1f;
    }
    /// <summary>
    /// Handles dive input and plays diving animations.
    /// </summary>
    public void Dive(bool divePressed)
    {
        if(divePressed)
        {
            whaleBody.mass += 300;
            this.GetComponent<SimpleBoyancy>().SetDensity(769.276f);

            if (transform.position.y > -4.0f)
            {
                DiveAnimation();
            }
            else
            {
                RoamAnimation();
            }
        }
        else
        {
            whaleBody.mass = 63.1f;
            this.GetComponent<SimpleBoyancy>().SetDensity(790f);
            RoamAnimation();
        }
    }
    /// <summary>
    /// Handles effects of speedup powerup.
    /// <param name="duration">THe duration of the speedup.</param>
    /// </summary>
    public void SpeedupPowerup(float duration, AudioClip sound)
    {
        AudioSource whaleAudio = GetComponent<AudioSource>();
        if (whaleAudio != null)
        {
            whaleAudio.PlayOneShot(sound, 1);
        }
        if (speedupDuration == 0)
        {
            speedupDuration = duration;
        }
        speedupStart = Time.time;
        if (spedup)
        {
            maxForwardSpeed *= 1.1f;
            maxBackwardSpeed *= 1.1f;
            turnSpeed *= 1.1f;
        }
        else
        {
            maxForwardSpeed *= 1.5f;
            maxBackwardSpeed *= 1.5f;
            turnSpeed *= 1.5f;
        }

        spedup = true;

    }
    /// <summary>
    /// Handles disabling of powerups after they're picked up.
    /// <param name="other">The pickup the player has collided with.</param>
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            // yield return new WaitForSeconds(2000);
        }
    }
    /// <summary>
    /// handles slowdown from oil slicks.
    /// </summary>
    public void slowDown()
    {
        maxForwardSpeed *= .2f;
        maxBackwardSpeed*= .2f;
    }
    /// <summary>
    /// Handles oil slick slowdown expiry.
    /// </summary>
    public void slowDownEnd()
    {
        maxForwardSpeed = baseMaxForward;
        maxBackwardSpeed = baseMaxBackward;
    }
    /// <summary>
    /// Repositions whale and adds force if out of constraints
    /// </summary>
    private void NoFlyingWhales()
    {
        if(this.transform.position.y > 3.5f)
        {
            Debug.Log("no flying whales");
            whaleBody.AddForce(Vector3.down * 2000.0f);// * Time.deltaTime);
        }
    }
    /// <summary>
    /// Handles bouncing the player away on collision with crates.
    /// </summary>
    /// <param name="direction">The direction to bounce</param>
    /// <param name="sound">The bounce sound.</param>
    public void BounceBack(Vector3 direction, AudioClip sound)
    {
		if (playerHealth.currentHealth > 0) {
			playerHealth.TakeDamage (10);
		}
        GetComponent<AudioSource>().PlayOneShot(sound);
        // Vector3 backward = transform.forward * -1;
        whaleBody.AddForce(direction * 100000);
        Debug.Log("BounceBack Called, " + transform.forward + ", direction , " + direction);
    }
}

