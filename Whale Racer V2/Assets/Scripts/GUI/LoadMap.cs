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
    /// <summary>
    /// Load the map selected in SelectedMap (by user using menu buttons)
    /// </summary>
    public void loadSelected()
    { 
        SceneManager.LoadScene(SelectedMap.selectedMap);
    }
    /// <summary>
    /// load the selected map's MP version.
    /// </summary>
    public void loadSelectedMP()
    {
        mpMap = SelectedMap.selectedMap += "_MP";
        SceneManager.LoadScene("MPLobby");
    }
    /// <summary>
    /// Load a map by name.
    /// </summary>
    /// <param name="sceneName">The name of the map to be loaded.</param>
	public void loadByName(string sceneName)
	{
        if (isStartSplitscreenButton){
            passSplitscreen = true;
        }
        SceneManager.LoadScene(sceneName);


    }
    /// <summary>
    /// Quit the game.
    /// </summary>
    public void quitGame()
    {
        Application.Quit();
    }

}
