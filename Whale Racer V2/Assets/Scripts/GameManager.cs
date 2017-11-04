using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gmInst;
    public static int winner;
    public int totalLaps = 1;
    public static float timer;
    public static List<string> finishTimes = new List<string>();
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
                PlayerManager.allWhaleMovementDisabled = false;
            }
        }
        else
        {
            timer += Time.deltaTime;
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

    public static string getStringTime()
    {
        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = (timer % 60).ToString("00");
        return minutes + ":" + seconds;

    }
}
