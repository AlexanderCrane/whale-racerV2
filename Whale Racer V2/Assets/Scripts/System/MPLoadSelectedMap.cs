using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine;

public class MPLoadSelectedMap : MonoBehaviour {
    private string mapName;
	// Use this for initialization
	void Start () {
        mapName = LoadMap.mpMap;
        if (mapName == null)
        {
            mapName = "Aduloo_MP";
        }
        gameObject.GetComponent<NetworkLobbyManager>().playScene = mapName;
        
	}
}
