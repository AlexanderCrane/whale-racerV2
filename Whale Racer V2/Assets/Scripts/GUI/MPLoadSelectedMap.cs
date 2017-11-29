using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine;
/// <summary>
/// Script to pull the selected map from LoadMap and pass it to the lobby manager
/// </summary>
public class MPLoadSelectedMap : MonoBehaviour {
    private string mapName;
    /// <summary>
    /// Start method. Sets the lobby manager's play scene to the selected map from the menu.
    /// </summary>
    void Start () {
        mapName = LoadMap.mpMap;
        if (mapName == null)
        {
            mapName = "Aduloo_MP";
        }
        gameObject.GetComponent<NetworkLobbyManager>().playScene = mapName;
        
	}
}
