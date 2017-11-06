using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///Player movement control script.
/// </summary>
public class PlayerMovement : MonoBehaviour {
    private float xMovement;
    private float zMovement;
    public Animator whaleAnimator;
    private AnimationHashTable animations;

    public float whaleSpeed = 0f;
    public float turnSpeed = 15f;
    public float baseTurnSpeed = 15f;
    public float accel = .5f;
    public float maxForwardSpeed = 17f;
    public float maxBackwardSpeed = 6f;

    public float baseMaxForward = 17f;
    public float baseMaxBackward = 6f;

    private int canJump = 0;
    private float speedupDuration = 0;
    private float speedupStart = 0;

    private bool diving = false;
    private bool spedup = false;
    private bool underWater = false;
    private bool movementAudioPlaying = false;
    private Rigidbody whaleBody;

    /// <summary>
    /// Awake method. Stores base values for forward/backward/turn speed. Initializes animator and rigidbody.
    /// </summary>
    private void Awake()
    {
        whaleBody = GetComponent<Rigidbody>();
        whaleAnimator = gameObject.GetComponent<Animator>();
        baseMaxForward = maxForwardSpeed;
        baseMaxBackward = maxBackwardSpeed;
        baseTurnSpeed = turnSpeed;
    }
    /// <summary>
    /// Update method. Calls methods to player input to whale movement.
    /// Checks for power up expiration and handles jump lockout.
    /// </summary>
    void FixedUpdate()
    {
        if (animations == null)
        {
            animations = GameManager.gmInst.GetComponent<AnimationHashTable>();
        }
        //cheat code - press N+M+RSHIFT to add a lap to your lap counter if you're testing the endgame
        if ((Input.GetKey(KeyCode.M) && Input.GetKey(KeyCode.N)) && Input.GetKeyDown(KeyCode.RightShift))
        {
            gameObject.GetComponent<PlayerManager>().pmInstance.NewLap();
        }

        xMovement = Input.GetAxis("Horizontal");
        zMovement = Input.GetAxis("Vertical");
        //lock y rotation to 0 so the whale can't be flipped over (for now)
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
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
        Move2D();
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
    /// <summary>
    /// Translates player input to forward/backward movement. Plays corresponding audio and animation.
    /// </summary>
    private void Move2D()
    {
        Vector3 movement = new Vector3(0.0f, 0.0f, zMovement);
        AudioSource movementSplash = GetComponent<AudioSource>();
        //going forward
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            if (!movementAudioPlaying)
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
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            if (!movementAudioPlaying)
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
            if (movementAudioPlaying)
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
        whaleBody.AddRelativeForce(0.0f, 0.0f, zMovement * (-whaleSpeed));
    }
    /// <summary>
    /// Translates input into rotation.
    /// </summary>
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
    /// <summary>
    /// Handles jump input.
    /// </summary>
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
    /// <summary>
    /// Handles dive input and plays diving animations.
    /// </summary>
    void Dive()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            //underWater = !underWater;
            /*if(this.transform.position.y > -3.0f) { underWater = false; }
            else { underWater = true; }*/

            underWater = !underWater;

            if (!underWater)
            {
                whaleBody.mass = 1000.0f;
                this.GetComponent<SimpleBoyancy>().SetDensity(769.276f);
                whaleAnimator.SetBool(animations.underwaterBool, false);

                whaleAnimator.SetBool(animations.diveBool, true);
                whaleAnimator.SetBool(animations.subMovementBool, true);
                whaleAnimator.speed = .5f;
                BGMController.bgmInst.ToggleUnderWaterSound();
                StartCoroutine("UnderWaterCoroutine");
            }
            else
            {
                whaleBody.mass = 63.1f;
                this.GetComponent<SimpleBoyancy>().SetDensity(790f);
                whaleAnimator.SetBool(animations.underwaterBool, true);
                whaleAnimator.SetBool(animations.diveBool, false);
                whaleAnimator.SetBool(animations.subMovementBool, false);
                BGMController.bgmInst.ToggleAboveWaterSound();

                whaleAnimator.speed = 1f;
            }
        }
        else
        {

            whaleAnimator.SetBool(animations.diveBool, false);
        }
    }

    /// <summary>
    /// Waits to stabilize whale buoyancy underwater
    /// </summary>
    IEnumerator UnderWaterCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            whaleBody.mass = 96.129f;
            Debug.Log("Underwater Now & Stable");
            StopAllCoroutines();
        }
        
    }
    /// <summary>
    /// Handles effects of speedup powerup.
    /// <param name="duration">THe duration of the speedup.</param>
    /// </summary>
    public void SpeedupPowerup(float duration)
    {
        if (speedupDuration == 0)
        {
            speedupDuration = duration;
        }
        spedup = true;
        speedupStart = Time.time;
        Debug.Log(maxForwardSpeed);
        maxForwardSpeed *= 1.5f;
        Debug.Log(maxForwardSpeed);
        maxBackwardSpeed *=1.5f;
        turnSpeed *=1.5f;
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

        baseMaxForward = 17f;
        baseMaxBackward = 6f;
    }

    public void BounceBack(Vector3 direction)
    {
       // Vector3 backward = transform.forward * -1;
        whaleBody.AddForce(direction * 100000);
        Debug.Log("BounceBack Called, " + transform.forward + ", direction , " + direction);
    }
}

