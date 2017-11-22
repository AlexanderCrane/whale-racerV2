using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestMovement : IPrebuildSetup {
    private GameObject whale;
    private PlayerMovement testPM;
    private Transform testTransform;

    [SetUp]
    public void Setup()
    {
        whale = GameObject.Find("Whale_Humpbac");
        testPM = whale.GetComponent<PlayerMovement>();
        testTransform = whale.transform;
        testPM.whaleAnimator = GameObject.Find("Whale_Humpbac").GetComponent<Animator>();
        testPM.animations = new AnimationHashTable();
    }
    [Test]
    [PrebuildSetup(typeof(TestMovement))]
    public void TestJumpCooldown() {

        testPM.Jump(true); //jump
        Assert.That(testPM.CheckCanJump() == false); //test that whale's ability to jump is set to false
	}

    [Test]
    [PrebuildSetup(typeof(TestMovement))]
    public void TestJump()
    {
        float initY = testTransform.position.y;
        testPM.Jump(true);
        System.Threading.Thread.Sleep(30); //jump and wait 30 milliseconds
        Assert.That(testTransform.position.y >= initY); //pass test if y position increases
    }

} 
