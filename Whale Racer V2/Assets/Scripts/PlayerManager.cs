using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {
    public int playerID = 0;
    public PlayerManager pmInstance = null;
    public int currentCheckpoint = 0;
    private int currentLap = 1;
    public bool[] checkpointsHit;
	// Use this for initialization
	void Awake () {
		if (pmInstance == null && pmInstance != this)
        {
            pmInstance = this;
        }
        checkpointsHit = new bool[12];
    }
    public void newLap()
    {
        Debug.Log("Lap" + currentLap + "Complete");
        this.currentLap++;
        currentCheckpoint = 0;
        for(int i =0; i<checkpointsHit.Length; i++)
        {
            checkpointsHit[i] = false;
        }
        //totalLaps is configurable number of laps for race to last
        if (currentLap == GameManager.gmInst.totalLaps+1)
        {
            GameManager.winner = playerID;
            GameManager.finishTimes.Add(GameManager.getStringTime());
            SceneManager.LoadScene(2);
        }
    }

// Update is called once per frame
    void Update () {
		
	}
}
