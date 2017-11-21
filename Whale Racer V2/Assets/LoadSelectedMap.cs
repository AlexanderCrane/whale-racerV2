using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine;

public class LoadSelectedMap : MonoBehaviour {
    private string mapName;
	// Use this for initialization
	void Start () {
        mapName = LoadMap.mpMap;
        gameObject.GetComponent<NetworkLobbyManager>().playScene = mapName;

	}
}
