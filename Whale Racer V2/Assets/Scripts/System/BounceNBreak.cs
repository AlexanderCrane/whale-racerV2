using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///  Script for breakable box objects.
/// </summary>
public class BounceNBreak : MonoBehaviour {

    [SerializeField] private Vector3 correctDirection;

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
        Debug.Log("Collision");
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Box collided with");
            PlayerMovement collidingPlayer = other.gameObject.GetComponent<PlayerMovement>();
            if (collidingPlayer == null)
            {
                Debug.Log("AI COLLIDE");
                other.gameObject.GetComponent<Rigidbody>().AddForce(correctDirection*10000);
            }
            collidingPlayer.BounceBack(correctDirection);
            Destroy(this.gameObject);

        }
    }
}
