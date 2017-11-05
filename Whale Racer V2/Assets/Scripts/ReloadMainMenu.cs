using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ReloadMainMenu : MonoBehaviour {

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