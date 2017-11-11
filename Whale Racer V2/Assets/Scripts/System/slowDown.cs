using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///Script to handle oil slick collision and slowdown.
/// </summary>
public class slowDown : MonoBehaviour
{
    /// <summary>
    /// Checks for collision with the player and calls PlayerMovement to apply slowdown debuff.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("hit");
            PlayerMovement collidingPlayer = other.GetComponent<PlayerMovement>();
            collidingPlayer.slowDown();


        }


    }
    /// <summary>
    /// Checks for end of collision with the player and expires the slowdown debuff.
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerMovement collidingPlayer = other.GetComponent<PlayerMovement>();
            collidingPlayer.slowDownEnd();


        }


    }
}