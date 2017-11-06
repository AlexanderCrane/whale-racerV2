using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// Script that the main menu from the endgame screen.
/// </summary>
public class ReloadMainMenu : MonoBehaviour {

    /// <summary>
    /// Waits five seconds after endgame screen load, then loads scene 0 (main menu).
    /// </summary>
    void Update()
    {
       // Debug.Log("update");
        Debug.Log(Time.timeSinceLevelLoad);
        if (Time.timeSinceLevelLoad > 5)
        {
            SceneManager.LoadScene(0);
        }
    }
}