using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Script to manage interaction between all types of powerups and players.
/// </summary>
public class PowerUp : MonoBehaviour {

    public bool isSpeedup;
    public float speedupDuration = 5;
    /// <summary>
    /// OnTriggerEnter method for powerups. Calls PlayerMovement to modify player state based on type of powerup.
    /// <param name="other">The object colliding with the PowerUp</param>
    /// </summary>
    void OnTriggerEnter (Collider other) {
        if (other.gameObject.tag == "Player")
        {
            if (isSpeedup)
            {
                PlayerMovement collectingPlayer = other.GetComponent<PlayerMovement>();
                Debug.Log("Collected powerup!");
                collectingPlayer.SpeedupPowerup(speedupDuration);
            }
        }
    }
}
