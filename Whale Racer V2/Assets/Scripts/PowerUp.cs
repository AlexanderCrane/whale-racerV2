using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    public bool isSpeedup;
    public bool isSlowed;
    public float speedupDuration = 5;
    public float slowDuration = 5; //LATER REMOVE THIS in favor of being slowed only while intersecting with oil


    // Use this for initialization
    void OnTriggerEnter (Collider other) {
        if (other.gameObject.tag == "Player")
        {
            if (isSpeedup)
            {
                PlayerMovement collectingPlayer = other.GetComponent<PlayerMovement>();
                Debug.Log("Collected powerup!");
                collectingPlayer.SpeedupPowerup(speedupDuration);
            }

            if (isSlowed)
            {
                PlayerMovement collectingPlayer = other.GetComponent<PlayerMovement>();
                Debug.Log("Entered Slow Zone!");
                collectingPlayer.SlowZone(slowDuration);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
