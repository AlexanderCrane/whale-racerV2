using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class LoadMap : MonoBehaviour {

    public bool isStartSplitscreenButton;
    public static bool passSplitscreen;
	public void loadByIndex(int sceneIndex)
	{
        if (isStartSplitscreenButton){
            passSplitscreen = true;
        }
		SceneManager.LoadScene (sceneIndex); 
	}
    public void quitGame()
    {
        Application.Quit();
    }
}
