using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class LoadMap : MonoBehaviour {

    public bool isStartSplitscreenButton;
    public static bool passSplitscreen;
    public static string mpMap;
    public void loadSelected()
    { 
        SceneManager.LoadScene(SelectedMap.selectedMap);
    }
    public void loadSelectedMP()
    {
        mpMap = SelectedMap.selectedMap += "_MP";
        SceneManager.LoadScene("MPLobby");
    }
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
