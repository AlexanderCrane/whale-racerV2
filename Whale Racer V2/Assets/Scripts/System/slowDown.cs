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
            NavMeshAgent computerPlayer = other.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();

            if (computerPlayer != null)
            {
                if (computerPlayer.isActiveAndEnabled)
                {
                    Debug.Log("AI entered slick");
                    computerPlayer.speed = 4;
                }
                else
                {
                    collidingPlayer.slowDown();
                }
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
            NavMeshAgent computerPlayer = other.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (computerPlayer != null)
            {
                if (computerPlayer.isActiveAndEnabled)
                {
                    computerPlayer.speed = 17;
                }
                else
                {
                    collidingPlayer.slowDownEnd();
                }
            }
            else
                collidingPlayer.slowDownEnd();

        }


    }
}