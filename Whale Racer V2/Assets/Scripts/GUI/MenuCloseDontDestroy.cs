using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A script to close DontDestroyOnLoad gameobjects on return to main menu.
/// </summary>
public class MenuCloseDontDestroy : MonoBehaviour {
    /// <summary>
    /// when we return from the lobby to the main menu, make sure the lobbymanager gets killed
    /// when we return from a game to the main menu, make sure the gamemanager gets killed
    /// </summary>
    void Awake()
    {
        if (GameObject.Find("LobbyManager") != null)
        {
            Destroy(GameObject.Find("LobbyManager"));
        }
        if (GameObject.Find("GameController") != null)
        {
            Debug.Log("Destroying");
            Destroy(GameObject.Find("GameController"));
        }
        if (GameObject.FindGameObjectsWithTag("LobbyPlayer") != null)
        {
            Debug.Log("Destroying");
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("LobbyPlayer"))
            {
                Destroy(obj);
            }
        }
    }
}
