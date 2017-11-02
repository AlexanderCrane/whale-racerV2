using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    public bool isSpeedup;
    public float speedupDuration = 5;
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
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
