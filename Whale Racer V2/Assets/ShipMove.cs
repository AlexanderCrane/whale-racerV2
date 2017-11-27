using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMove : MonoBehaviour {

    public static int movespeed = 1;
    public Vector3 forwardDirection = Vector3.forward;
    public int i = 0;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (i < 10)
        {
            transform.Translate(forwardDirection * movespeed * Time.deltaTime);
            i++;
        }
        if (i == 10)
        {
            transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
            i = 0;
        }

    }
}
