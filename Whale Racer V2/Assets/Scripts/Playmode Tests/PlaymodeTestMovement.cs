using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections;

public class NewPlayModeTest : IPrebuildSetup {
    private GameObject whale;
    private PlayerMovement testPM;
    private Transform testTransform;


    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene("Aduloo");
    }
    //after running nunit's setup function to load the scene we need to yield null to run for a frame
    //if we do not yield, the gameobjects we're testing with won't be initialized
    //we can only do this in the actual UnityTests, so call this 'real setup' function after we do
    //this does the actual setup
    //yes this is a hack
    public void RealSetup()
    {
        whale = GameObject.Find("Whale_Humpbac");
        testPM = whale.GetComponent<PlayerMovement>();
        testTransform = whale.transform;
        testPM.whaleAnimator = GameObject.Find("Whale_Humpbac").GetComponent<Animator>();
        testPM.animations = new AnimationHashTable();
    }
    [UnityTest]
	public IEnumerator TestJumpIncreasesHeight() {
        //yield a frame before initializing game objects to let the scene load
        yield return null;
        RealSetup();

        float initY = whale.transform.position.y;
        testPM.SetCanJump();
        testPM.Jump(true);
        yield return null; //yield for three frames to let the jump take effect
        yield return null;
        yield return null;

        Assert.That(whale.transform.position.y > initY); //pass test if y position increases

    }
}
