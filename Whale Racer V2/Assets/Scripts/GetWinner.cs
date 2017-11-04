using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWinner : MonoBehaviour
{

    /// <summary>
    /// Awake method. Loads the winner and their time from the previous scene's Game Manager and displays it.
    /// Will eventually display all finish times?
    /// </summary>
    void Awake()
    {
        string winnerTime = GameManager.finishTimes[0];
        gameObject.GetComponent<TextMesh>().text = "Player " + GameManager.winner.ToString() + " wins in " + winnerTime + "!";

    }
}
