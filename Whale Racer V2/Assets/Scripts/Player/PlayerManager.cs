using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

///  <summary>
///  Tracks progress through the race and global race effects for each individual player.
///  </summary>
public class PlayerManager : MonoBehaviour {
    public int playerID = 0;
    public PlayerManager pmInstance = null;
    public int currentCheckpoint = 0;
    private int currentLap = 1;
    public bool[] checkpointsHit;
    //all whale movement is disabled at start of game
    //when the race countdown ends, GameManager sets this to false
    public static bool allWhaleMovementDisabled = true;

    /// <summary>
    /// Awake method. Initializes instance for non-static use. Initializes checkpointsHit array.
    /// </summary>
    void Awake () {
		if (pmInstance == null && pmInstance != this)
        {
            pmInstance = this;
        }
        checkpointsHit = new bool[12];

    }
    /// <summary>
    /// Called when the player hits the finish line. Adds a lap or ends the race depending on the GameManager's total laps.
    /// </summary>
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

    /// <summary>
    /// Update method for the player manager. Disables player movement if countdown isn't over yet.
    /// </summary>
    void Update (){
        NavMeshAgent nma = gameObject.GetComponent<NavMeshAgent>();
        if (allWhaleMovementDisabled)
        {
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

            if (nma != null && nma.isActiveAndEnabled)
            {
                gameObject.GetComponent<NavMeshAgent>().isStopped = true;
            }      
        }
        else
        {
            if (nma != null && nma.isActiveAndEnabled)
            {
                gameObject.GetComponent<NavMeshAgent>().isStopped = false;
            }
        }
    }
}
