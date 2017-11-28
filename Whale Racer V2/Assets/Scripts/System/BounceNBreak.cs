using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///  Script for breakable box objects.
/// </summary>
public class BounceNBreak : MonoBehaviour {

    [SerializeField] private Vector3 correctDirection;
    public AudioClip sound;

	/// <summary>
    /// Start method. Sets the direction to bounce the player on collision
    /// </summary>
	void Start () {
        correctDirection = transform.forward;
	}
    /// <summary>
    /// OnCollisionEnter method. Bounces the colliding object back if it's the player.
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collision with " + other.gameObject.name);
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Box collided with");
            PlayerMovement collidingPlayer = other.gameObject.GetComponent<PlayerMovement>();

            collidingPlayer.BounceBack(correctDirection, sound);
            if (GameManager.gmInst.isMP)
            {
                UnityEngine.Network.Destroy(this.gameObject);
                Destroy(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }

        }
    }
}
