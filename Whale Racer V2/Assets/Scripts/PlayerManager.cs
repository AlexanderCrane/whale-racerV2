using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {
    public int playerID = 0;
    public PlayerManager pmInstance = null;
    public int currentCheckpoint = 0;
    private int currentLap = 1;
    public bool[] checkpointsHit;
    //all whale movement is disabled at start of game
    //when the race countdown ends, GameManager sets this to false
    public static bool allWhaleMovementDisabled = true;

	// Use this for initialization
	void Awake () {
		if (pmInstance == null && pmInstance != this)
        {
            pmInstance = this;
        }
        checkpointsHit = new bool[12];

    }
    public void NewLap()
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
    void Update (){
        if (allWhaleMovementDisabled)
        {
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

            if (gameObject.GetComponent<NavMeshAgent>() != null)
            {
                gameObject.GetComponent<NavMeshAgent>().isStopped = true;
            }      
        }
        else
        {
            if (gameObject.GetComponent<NavMeshAgent>() != null)
            {
                gameObject.GetComponent<NavMeshAgent>().isStopped = false;
            }
        }
    }
}
