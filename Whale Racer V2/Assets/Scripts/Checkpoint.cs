using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {
    public int position;
    public bool checkpointHit = false;

    void Awake()
    {
        RaceManager.rmInstance.checkPoints.Add(this);
    }
	// Update is called once per frame
	void Update () {
	}
    void OnTriggerEnter(Collider other)
    {
        RaceManager manager = RaceManager.rmInstance;
        if (!checkpointHit)
        {
            manager.currentCheckpoint = position;
            Debug.Log("Checkpoint " + manager.currentCheckpoint);
            checkpointHit = true;
            if (position == -1)
            {
                manager.newLap();
            }
        }

    }

}
