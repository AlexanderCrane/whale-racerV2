using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections;
using System.Linq;
/// <summary>
/// Tests for the checkpoint system and winning the race.
/// </summary>
public class TestCheckpoints : IPrebuildSetup
{
    private GameObject whale;
    private PlayerManager testManager;
    private Transform testTransform;

    /// <summary>
    /// Loads Aduloo.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene("Aduloo");
    }
    /// <summary>
    /// RealSetup method to be run after Aduloo loads.
    /// </summary>
    public void RealSetup()
    {
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        gm.countdownLength = 0;
        whale = GameObject.Find("Whale_Humpbac");
        testManager = whale.GetComponent<PlayerManager>();
    }
    /// <summary>
    /// Test that hitting the first checkpoint is recorded in the whale's PlayerManager
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator TestCheckpointHitRecorded()
    {
        //yield a frame before initializing game objects to let the scene load
        yield return null;
        RealSetup();

        GameObject checkpoint1 = GameObject.Find("Checkpoint 1");
        whale.transform.position = checkpoint1.transform.position; //teleport whale to checkpoint 1, verify that playermanager marks checkpoint 1 as hit
        yield return null;
        Assert.That(testManager.checkpointsHit[0] == true);
    }
    /// <summary>
    /// Test that hitting the second checkpoint without hitting the first does nothing.
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator TestHitCheckpointOutOfOrderFails()
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
    /// <summary>
    /// Test that hitting the first checkpoint twice doesn't mark checkpoint 2 as visited.
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator TestHitCheckpointRepeatedlyFails()
    {
        yield return null;
        RealSetup();
        GameObject checkpoint1 = GameObject.Find("Checkpoint 1");
        whale.transform.position = checkpoint1.transform.position; //teleport whale to checkpoint 1
        yield return null;

        whale.transform.position = new Vector3(0, 0, 0) ; //teleport whale to an arbitrary point
        yield return null;

        whale.transform.position = checkpoint1.transform.position; //and back
        yield return null;

        Assert.That(testManager.checkpointsHit.Where(q => q == true).Count() == 1); //verify that only one checkpoint is still marked
    }
    /// <summary>
    /// Test that, when FinishLine is the next checkpoint the player needs to hit, hitting it ends the game.
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator TestHittingFinishWinsGame()
    {
        yield return null;
        RealSetup();
        GameObject finishLine = GameObject.Find("FinishLine");
        for(int i =0; i<testManager.checkpointsHit.Length; i++) //test player has hit all checkpoints except the finish line
        {
            if (i!= testManager.checkpointsHit.Length - 1)
            {
                testManager.checkpointsHit[i] = true;
            }
        }
        testManager.currentCheckpoint = finishLine.GetComponent<Checkpoint>().position - 1;
        whale.transform.position = finishLine.transform.position; //teleport to finish line
        for (int i = 0; i < 200; i++)
        {
            yield return null;
        }
        Assert.That(SceneManager.GetActiveScene().name == "EndGame");
    }
}
