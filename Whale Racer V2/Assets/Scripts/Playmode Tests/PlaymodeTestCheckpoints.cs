using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections;
using UnityEditor.SceneManagement;

public class TestCheckpoints : IPrebuildSetup
{
    private GameObject whale;
    private PlayerManager testManager;
    private Transform testTransform;


    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene("Aduloo");
    }

    public void RealSetup()
    {
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        gm.countdownLength = 0;
        whale = GameObject.Find("Whale_Humpbac");
        testManager = whale.GetComponent<PlayerManager>();
    }

    [UnityTest]
    public IEnumerator TestCheckpointHit()
    {
        //yield a frame before initializing game objects to let the scene load
        yield return null;
        RealSetup();

        GameObject checkpoint1 = GameObject.Find("Checkpoint 1");
        whale.transform.position = checkpoint1.transform.position; //teleport whale to checkpoint 1, verify that playermanager marks checkpoint 1 as hit
        yield return null;
        Assert.That(testManager.checkpointsHit[0] == true);
    }
    [UnityTest]
    public IEnumerator TestHitCheckpointOutOfOrder()
    {
        yield return null;
        RealSetup(); //teleport whale to checkpoint 2 without hitting checkpoint 1,verify not marked
        GameObject checkpoint2 = GameObject.Find("Checkpoint 2");
        whale.transform.position = checkpoint2.transform.position;
        yield return null;
        foreach(bool checkpointHit in testManager.checkpointsHit)
        {
            Assert.That(checkpointHit == false);
        }
           
    }

}
