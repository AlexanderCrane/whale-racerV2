using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gmInst;
    public static int winner;
    public int totalLaps = 1;
    public static float timer;
    public static List<string> finishTimes = new List<string>();
    void Awake()
    {
        if (gmInst == null)
        {
            gmInst = this;
        }
    }
    void Update()
    {
        timer += Time.deltaTime;
    }
    void OnGUI()
    {
        GUI.color = Color.black;
        GUI.Label(new Rect(550, 10, 250, 100), getStringTime());
    }
    public static string getStringTime()
    {
        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = (timer % 60).ToString("00");
        return minutes + ":" + seconds;

    }
}
