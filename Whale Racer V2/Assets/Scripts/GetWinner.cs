using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWinner : MonoBehaviour
{

    // Use this for initialization
    void Awake()
    {
        gameObject.GetComponent<TextMesh>().text = "Player " +  GameManager.winner.ToString() + " wins!";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
