using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections;
using UnityEngine.AI;

public class TestAI : IPrebuildSetup {
    private GameObject whale;
    private NavMeshAgent testNMA;
    private MoveTo testMoveTo;
    private Transform testTransform;


    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene("Aduloo");
    }
    //after running nunit's setup function to load the scene we need to yield to run for a frame
    //if we do not yield, the gameobjects we're testing with won't be initialized
    //we can only yield in the actual UnityTests, so call this 'real setup' function after we do
    //this does the actual setup
    //yes this is a hack
    public void RealSetup()
    {
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        gm.countdownLength = 0;
        whale = GameObject.Find("AI_Whale");
        testNMA = whale.GetComponent<NavMeshAgent>();
        testMoveTo = whale.GetComponent<MoveTo>();
        testTransform = whale.transform;
    }
    [UnityTest]
    public IEnumerator TestAIWaypointCollideChangesTarget()
    {
        //yield a frame before initializing game objects to let the scene load
        yield return null;
        RealSetup();
        PathingWaypoint waypoint1 = GameObject.Find("Checkpoint 1").GetComponent<PathingWaypoint>();
        GameObject waypoint2 = GameObject.Find("Waypoint 1.5");
        //ocean will move the checkpoint slightly from where it is set, so check that we're close
        Assert.That(Vector3.Distance(testNMA.destination, waypoint1.transform.position) <10);
        whale.transform.position = waypoint1.transform.position;
        yield return null;
        Assert.That(Vector3.Distance(testNMA.destination, waypoint2.transform.position) < 10);
    }
}
