using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
            PlayerMovement collidingPlayer = other.GetComponent<PlayerMovement>();
            Debug.Log("Entered slick");
            if (collidingPlayer.gameObject.GetComponent<NavMeshAgent>().isActiveAndEnabled)
            {
                Debug.Log("AI entered slick");
                NavMeshAgent computerPlayer = other.gameObject.GetComponent<NavMeshAgent>();
                computerPlayer.speed = 4;
            }
            else
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

            if (collidingPlayer.ToString() == "AI_Whale (PlayerMovement)")
            {
                UnityEngine.AI.NavMeshAgent computerPlayer = other.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
                computerPlayer.speed = 17;
            }
            else
                collidingPlayer.slowDownEnd();

        }


    }
}