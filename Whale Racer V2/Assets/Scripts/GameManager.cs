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
    void Awake()
    {
        if (gmInst == null)
        {
            gmInst = this;
        }
    }
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
    void OnGUI()
    {
        GUIStyle style = new GUIStyle
        {
            font = Resources.Load<Font>("BAUHS93"),
            fontSize = 40,
        };
        style.normal.textColor = Color.red;
        if (countdownOngoing)
        {
            GUI.Label(new Rect(910, 300, 1, 1), countdownTime.ToString().Substring(0, 4), style);
        }
        if (showGo)
        {
            GUI.Label(new Rect(910, 300, 1, 1), "GO!", style);
            if(Time.timeSinceLevelLoad > 8f)
            {
                showGo = false;
            }
        }
        GUI.Label(new Rect(1700, 20, 250, 100), getStringTime(), style);
        //asdf
    }
    public static string getStringTime()
    {
        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = (timer % 60).ToString("00");
        return minutes + ":" + seconds;

    }
}
