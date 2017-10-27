using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour {
    public static RaceManager rmInstance = null;
    public int currentCheckpoint = 0;
    private int currentLap = 1;
    public int totalLaps = 1;
    public List<Checkpoint> checkPoints;
	// Use this for initialization
	void Awake () {
		if (rmInstance == null)
        {
            rmInstance = this;
        }
        if(checkPoints == null)
        {
            checkPoints = new List<Checkpoint>();
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
        if (currentLap == totalLaps+1)
        {
            SceneManager.LoadScene(2);
        }
    }

// Update is called once per frame
    void Update () {
		
	}
}
