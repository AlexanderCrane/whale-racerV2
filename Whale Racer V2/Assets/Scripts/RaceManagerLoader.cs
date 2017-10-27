using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManagerLoader : MonoBehaviour {
    public GameObject raceManager;
	// Use this for initialization
	void Awake () {
		if(RaceManager.rmInstance == null)
        {
            Debug.Log("instantiating");
            Instantiate(raceManager);
            Debug.Log(RaceManager.rmInstance == null);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
