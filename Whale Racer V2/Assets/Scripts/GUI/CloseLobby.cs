using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A script to close the MP Lobby manager on return to main menu.
/// </summary>
public class CloseLobby : MonoBehaviour {
    /// <summary>
    /// when we return from the lobby to the main menu, make sure the lobbymanager gets killed
    /// </summary>
    void Awake()
    {
        if (GameObject.Find("LobbyManager") != null)
        {
            Destroy(GameObject.Find("LobbyManager"));
        }
    }
}
