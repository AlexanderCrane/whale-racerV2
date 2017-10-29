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
    // Update is called once per frame
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
        Debug.Log("drawing gui");
        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = (timer % 60).ToString("00");
        GUI.Label(new Rect(550, 10, 250, 100), minutes + ":" + seconds);


    }
}
