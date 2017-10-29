using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {
    public int position;
    public bool checkpointHit = false;

    void Awake()
    {
    }
	// Update is called once per frame
	void Update () {
	}
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameObject player = other.gameObject;
            PlayerManager manager = player.GetComponent<PlayerManager>();
            //PlayerManager manager = managerScript.pmInstance;
            if (!checkpointHit)
            {
                manager.currentCheckpoint = position;
                checkpointHit = true;
                if (position == -1)
                {
                    manager.newLap();
                }
            }
        }

    }

}
