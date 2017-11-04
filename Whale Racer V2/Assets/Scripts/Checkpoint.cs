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
            int index = position - 1;
            //can't hit checkpoints repeatedly or hit them out of order
            if (!manager.checkpointsHit[index] && (manager.currentCheckpoint == position-1))
            {
                Debug.Log(player.name + " hit " + this.gameObject.name);
                manager.currentCheckpoint = position;
                manager.checkpointsHit[index] = true;
                if (position == 12)
                {
                    manager.NewLap();
                }
            }
        }

    }

}
