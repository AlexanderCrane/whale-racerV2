using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using UnityEngine.Networking;

public class MPGameManager : NetworkBehaviour
{
    public static MPGameManager gmInst;
    public static int winner;
    public int totalLaps = 1;
    [SyncVar]
    public float timer;
    public static List<string> finishTimes = new List<string>();
    public static List<Camera> allPlayerCams = new List<Camera>();
    public float countdownLength;
    private float countdownTime;
    private bool countdownOngoing = true;
    private bool showGo = false;
    ///  <summary>
    /// Awake method. Instantiates the Game Manager for non-static use.
    ///  </summary>

    void Awake()
    {
        if (gmInst == null)
        {
            gmInst = this;
        }
        //some spicy LINQ for you
        //get a GameObject[] of all the objects tagged player camera, then map to a List<Camera> using the function .GetComponent<Camera>
        Camera[] cams = GameObject.FindGameObjectsWithTag("PlayerCamera").Select(obj => obj.GetComponent<Camera>()).ToArray();

        //if (LoadMap.passSplitscreen)
        //{
        //    if (cams.Count() == 2)
        //    {
        //        cams[0].rect = new Rect(0, 0, 1, .5f);
        //        cams[1].rect = new Rect(0, .5f, 1, .5f);
        //        GameObject ai = cams[0].GetComponent<lerpCamera>().target.gameObject;
        //        ai.GetComponent<NavMeshAgent>().enabled = false;
        //        ai.GetComponent<PlayerMovement>().enabled = true;
        //    }
        //}
        //else
        //{
        //    //if we're not doing splitscreen, disable all cameras that aren't player 1's camera
        //    cams.Where(cam => cam.name != "player_Camera").Select(cam => { cam.enabled = false; return cam; }).ToList();
        //    cams[0].GetComponent<lerpCamera>().target.transform.localScale = new Vector3(.5f, .5f, -.5f);
        //}
    }
    ///  <summary>
    /// For the first few seconds of the game, manages the initial countdown timer.
    ///  </summary>

    void Update()
    {
        if (countdownOngoing)
        {
            countdownTime = countdownLength - Time.timeSinceLevelLoad;
            if (countdownTime <= 0f)
            {
                countdownOngoing = false;
                showGo = true;
                MPPlayerManager.allWhaleMovementDisabled = false;
            }
        }
        else
        {
            if (NetworkServer.active)
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
           // Debug.Log(Screen.width);
            GUI.Label(new Rect(Screen.width/2, Screen.height/2-50, 1, 1), countdownTime.ToString().Substring(0, 4), style);
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

    public string getStringTime()
    {
        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = (timer % 60).ToString("00");
        return minutes + ":" + seconds;

    }
}
