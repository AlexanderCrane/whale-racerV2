using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gmInst;
    public static int winner;
    public int totalLaps = 1;
    // Update is called once per frame
    void Awake()
    {
        if (gmInst == null)
        {
            gmInst = this;
        }
    }
    void Update()
    {

    }
}
