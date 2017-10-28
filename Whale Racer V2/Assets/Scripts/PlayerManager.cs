using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {
    public int playerID = 0;
    public PlayerManager pmInstance = null;
    public int currentCheckpoint = 0;
    private int currentLap = 1;
    public UnityEngine.Object[] checkPoints;
	// Use this for initialization
	void Awake () {
		if (pmInstance == null)
        {
            pmInstance = this;
        }
        if(checkPoints == null)
        {
            checkPoints = FindObjectsOfType(typeof(Checkpoint));
        }
    }
    public void newLap()
    {
        Debug.Log("Lap" + currentLap + "Complete");
        this.currentLap++;
        foreach (Checkpoint point in checkPoints)
        {
            point.checkpointHit = false;
        }
        //totalLaps is configurable number of laps for race to last
        if (currentLap == GameManager.gmInst.totalLaps+1)
        {
            GameManager.winner = playerID;
            SceneManager.LoadScene(2);
        }
    }

// Update is called once per frame
    void Update () {
		
	}
}
