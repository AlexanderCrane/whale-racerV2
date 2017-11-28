using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

///  <summary>
///  Tracks progress through the race and global race effects for each individual player.
///  </summary>
public class PlayerManager : NetworkBehaviour {
    public int playerID = 0;
    public PlayerManager pmInstance = null;
    [SyncVar]
    public int currentCheckpoint = 0;
    private int currentLap = 1;
    public bool[] checkpointsHit;
    private bool hasCam = false;
    public MPLerpCamera cam;
    //all whale movement is disabled at start of game
    //when the race countdown ends, GameManager sets this to false
    public static bool allWhaleMovementDisabled = true;

    /// <summary>
    /// Awake method. Initializes instance for non-static use. Initializes checkpointsHit array.
    /// </summary>
    void Awake () {
        if (playerID == 0)
        {
            foreach(PlayerManager player in Object.FindObjectsOfType<PlayerManager>())
            {
                if (player.playerID != 0)
                {
                    playerID++;
                }
            }
            playerID += 1;
        }
        if (pmInstance == null && pmInstance != this)
        {
            pmInstance = this;
        }
        int numCheckpoints = Object.FindObjectsOfType<Checkpoint>().Count();
        checkpointsHit = new bool[numCheckpoints];

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
            GameManager.gmInst.winner = playerID;
            if (GameManager.gmInst.isMP)
            {
                GameManager.gmInst.finishTimes.Add(GameManager.getStringTime());
            }
            else
            {
                GameManager.gmInst.spFinishTimes.Add(GameManager.getStringTime());
            }
            SceneManager.LoadScene(2);
            Destroy(GameManager.gmInst);
        }
    }

    /// <summary>
    /// Update method for the player manager. Disables player movement if countdown isn't over yet.
    /// </summary>
    void Update (){
        if (GameManager.gmInst.isMP && !hasCam)
        {
            cam = FindObjectsOfType<MPLerpCamera>().Where(obj => obj.hasTarget() == false).First();
            cam.setTarget(this.gameObject);
            hasCam = true;
        }
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
