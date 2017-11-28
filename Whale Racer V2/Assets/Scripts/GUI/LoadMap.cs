using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Script containing various methods by which the main menu loads maps.
/// </summary>
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
        if (sceneName == "MainMenu" )
        {
            NetworkLobbyManager.singleton.StopClient();
            Destroy(GameObject.Find("LobbyManager"));
        }
		SceneManager.LoadScene(sceneName); 
	}
    public void quitGame()
    {
        Application.Quit();
    }

}
