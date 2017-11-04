using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slowDown : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerMovement collidingPlayer = other.GetComponent<PlayerMovement>();
            collidingPlayer.slowDown();


        }


    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerMovement collidingPlayer = other.GetComponent<PlayerMovement>();
            collidingPlayer.slowDownEnd();


        }


    }
}