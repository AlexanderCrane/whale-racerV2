using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {
    public int position;
    public bool checkpointHit = false;
    public bool isFinishLine;

    ///  <summary>
    /// OnTriggerEnter method. When a player enters the checkpoint, updates their state accordingly.
    /// Triggers new lap code in the game manager if the checkpoint is the finish line.
    ///  </summary>

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameObject player = other.gameObject;
            if (player.GetComponent<PlayerManager>() == null)
            {
                Debug.Log("NOT WHALE");
                player = player.GetComponent<ChildCollide>().collisionParent;
            }
            PlayerManager manager = player.GetComponent<PlayerManager>().pmInstance;
            int index = position - 1;
            //can't hit checkpoints repeatedly or hit them out of order
            if (!manager.checkpointsHit[index] && (manager.currentCheckpoint == position-1))
            {
                Debug.Log(player.name + " hit " + this.gameObject.name);
                manager.currentCheckpoint = position;
                manager.checkpointsHit[index] = true;
                if (isFinishLine)
                {
                    manager.NewLap();
                }
            }
        }

    }

}
