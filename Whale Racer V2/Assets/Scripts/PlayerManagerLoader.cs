using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerLoader : MonoBehaviour {
    public GameObject playerManager;
	// Use this for initialization
	void Awake () {
		if(gameObject.GetComponent<PlayerManager>().pmInstance == null)
        {
            Debug.Log("instantiating");
            Instantiate(playerManager);
            Debug.Log(gameObject.GetComponent<PlayerManager>().pmInstance == null);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
