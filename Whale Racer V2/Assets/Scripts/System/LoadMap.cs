using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class LoadMap : MonoBehaviour {

    public bool isStartSplitscreenButton;
    public static bool passSplitscreen;
	public void loadByName(string sceneName)
	{
        if (isStartSplitscreenButton){
            passSplitscreen = true;
        }
		SceneManager.LoadScene(sceneName); 
	}
    public void quitGame()
    {
        Application.Quit();
    }
}
