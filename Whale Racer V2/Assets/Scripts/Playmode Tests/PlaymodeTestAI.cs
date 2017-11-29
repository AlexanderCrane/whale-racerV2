using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections;
using UnityEngine.AI;
/// <summary>
/// Tests for the NavMeshAgent AI.
/// </summary>
public class TestAI : IPrebuildSetup {
    private GameObject whale;
    private NavMeshAgent testNMA;
    private MoveTo testMoveTo;
    private Transform testTransform;

    /// <summary>
    /// Loads Aduloo.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene("Aduloo");
    }
    [TearDown]
    public void Teardown()
    {
        UnityEngine.Object.Destroy(GameObject.FindGameObjectWithTag("GameController"));
    }
    /// <summary>
    /// Setup to run after Aduloo loads.
    /// </summary>
    public void RealSetup()
    {
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        gm.countdownLength = 0;
        whale = GameObject.Find("AI_Whale");
        testNMA = whale.GetComponent<NavMeshAgent>();
        testMoveTo = whale.GetComponent<MoveTo>();
        testTransform = whale.transform;
    }
    /// <summary>
    /// Check that the AI colliding with its current target waypoint gives it a new target.
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator TestAIWaypointCollideChangesTarget()
    {
        //yield a frame before initializing game objects to let the scene load
        yield return null;
        RealSetup();
        PathingWaypoint waypoint1 = GameObject.Find("Checkpoint 1").GetComponent<PathingWaypoint>();
        GameObject waypoint2 = GameObject.Find("Waypoint 1.5");
        //ocean will move the checkpoint slightly from where it is set, just check that we're close
        Assert.That(Vector3.Distance(testNMA.destination, waypoint1.transform.position) <10);
        whale.transform.position = waypoint1.transform.position; //teleport to waypoint 1
        yield return null; //check that the destination gets set to waypoint 2
        Assert.That(Vector3.Distance(testNMA.destination, waypoint2.transform.position) < 10);
    }
}
