using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWinner : MonoBehaviour
{

    // Use this for initialization
    void Awake()
    {
        string winnerTime = GameManager.finishTimes[0];
        gameObject.GetComponent<TextMesh>().text = "Player " + GameManager.winner.ToString() + " wins in " + winnerTime + "!";

    }

    // Update is called once per frame
    void Update()
    {

    }
}
