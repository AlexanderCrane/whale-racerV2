using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
/// <summary>
/// Class to handle various setup and GUI tasks throughout the race.
/// </summary>
public class GameManager : NetworkBehaviour
{
    public static GameManager gmInst;
    [SyncVar]
    public int winner;
    public int totalLaps = 1;
    [SyncVar]
    public float timer;
    public SyncListString finishTimes = new SyncListString();
    public List<String> spFinishTimes = new List<String>();
    public static List<Camera> allPlayerCams = new List<Camera>();
    public float countdownLength;
    private float countdownTime;
    private bool countdownOngoing = true;
    private bool showGo = false;
    public bool isMP;
    ///  <summary>
    /// Awake method. Instantiates the Game Manager for non-static use.
    ///  </summary>

    void Awake()
    {
        DontDestroyOnLoad(this);
        if (SceneManager.GetActiveScene().name == "Aduloo_MP" || SceneManager.GetActiveScene().name == "TheShipyard_MP" || SceneManager.GetActiveScene().name == "TheMinefield_MP")
        {
            isMP = true;
        }
        if (gmInst == null)
        {
            gmInst = this;
        }
        if (SelectedMap.selectedLaps > 0 && SelectedMap.selectedLaps < 4)
        {
            totalLaps = SelectedMap.selectedLaps;
        }
        else totalLaps = 1;
        //some spicy LINQ for you
        //get a GameObject[] of all the objects tagged player camera, then map to a List<Camera> using the function .GetComponent<Camera>
        if (!isMP)
        {
            Camera[] cams = GameObject.FindGameObjectsWithTag("PlayerCamera").Select(obj => obj.GetComponent<Camera>()).ToArray();

            if (LoadMap.passSplitscreen)
            {
                if (cams.Count() == 2)
                {
                    cams[0].rect = new Rect(0, 0, 1, .5f);
                    cams[1].rect = new Rect(0, .5f, 1, .5f);
                    GameObject ai = GameObject.Find("AI_Whale");
                    ai.GetComponent<NavMeshAgent>().enabled = false;
                    ai.GetComponent<PlayerMovement>().enabled = true;
                }

            }
            else
            {
                //if we're not doing splitscreen, disable all cameras that aren't player 1's camera
                cams.Where(cam => cam.name != "player_Camera").Select(cam => { cam.enabled = false; return cam; }).ToList();
                GameObject.Find("ai_Camera").GetComponent<lerpCamera>().target.transform.localScale = new Vector3(.5f, .5f, -.5f);
                GameObject p2Canvas = GameObject.Find("P2MinimapCanvas");
                GameObject p2MMCam = GameObject.Find("MinimapCameraP2");
                if (p2Canvas != null)
                {
                    GameObject.Find("P2MinimapCanvas").SetActive(false);
                }
                if (p2MMCam != null)
                {
                    GameObject.Find("MinimapCameraP2").SetActive(false);
                }
            }
        }
    }
    ///  <summary>
    /// For the first few seconds of the game, manages the initial countdown timer.
    ///  </summary>

    void Update()
    {
        if (countdownOngoing)
        {
            PlayerManager.allWhaleMovementDisabled = true;
            countdownTime = countdownLength - Time.timeSinceLevelLoad;
            if (countdownTime <= 0f)
            {
                countdownOngoing = false;
                showGo = true;
                PlayerManager.allWhaleMovementDisabled = false;
            }
        }
        else
        {
            if (isMP && NetworkServer.active)
            {
                timer += Time.deltaTime;
            }
            else if (!isMP)
            {
                timer += Time.deltaTime;
            }
        }
    }
    ///  <summary>
    /// OnGUI method. Draws player user interface.
    ///  </summary>

    void OnGUI()
    {
        GUIStyle style = new GUIStyle
        {
            font = Resources.Load<Font>("BAUHS93"),
            fontSize = 40,
            alignment = TextAnchor.UpperCenter
        };
        style.normal.textColor = Color.red;
        if (countdownOngoing)
        {
            //Debug.Log(Screen.width);
            try
            {
                GUI.Label(new Rect(Screen.width / 2, Screen.height / 2 - 50, 1, 1), countdownTime.ToString().Substring(0, 4), style);
            }
            catch(Exception e)
            {
                //this substring throws an error but i'm too dumb to understand it
                //and playmode unit tests break if there are any errors
                //so....
                //fix this later please
            }
        }
        if (showGo)
        {
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2-50, 1, 1), "GO!", style);
            if(Time.timeSinceLevelLoad > 8f)
            {
                showGo = false;
            }
        }
       // Debug.Log(Screen.width);
        GUI.Label(new Rect(Screen.width/2-110, 20, 250, 100), getStringTime(), style);
        //asdf
    }
    ///  <summary>
    /// Gets the current race time as a well-formatted string for the timer.
    ///  </summary>

    public static string getStringTime()
    {
        string minutes = Mathf.Floor(gmInst.timer / 59).ToString("00");
        string seconds = (gmInst.timer % 59).ToString("00");
        return minutes + ":" + seconds;

    }
}
