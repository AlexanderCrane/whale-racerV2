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
    //after running nunit's setup function to load the scene we need to yield to run for a frame
    //if we do not yield, the gameobjects we're testing with won't be initialized
    //we can only yield in the actual UnityTests, so call this 'real setup' function after we do
    //this does the actual setup
    //yes this is a hack
    public void RealSetup()
    {
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        gm.countdownLength = 0;
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

        float initY = whale.transform.position.y; //save whale's initial height and jump
        testPM.SetCanJump();
        testPM.Jump(true);

        //run for three frames to let jump take effect
        for (int i = 0; i < 3; i++)
        {
            yield return null;
        }

        Assert.That(whale.transform.position.y > initY); //pass test if y position increases
    }
    [UnityTest]
    public IEnumerator TestDiveModifiesHeight()
    {
        yield return null;
        RealSetup();

        float initY = whale.transform.position.y;
        testPM.Dive(true); //calling Dive while above water decreases whale height
        for (int i =0; i<100; i++)
        {
            yield return null;
        }
        float postDiveY = whale.transform.position.y;
        Assert.That(postDiveY< initY);
        testPM.Dive(true); //calling Dive while below water increases whale height
        for (int i = 0; i < 20; i++)
        {
            yield return null;
        }
        Assert.That(whale.transform.position.y > postDiveY);

    }
    [UnityTest]
    public IEnumerator TestMove()
    {
        yield return null;
        RealSetup();
        float initZ = System.Math.Abs(whale.transform.position.z);
        for (int i = 0; i < 5; i++)
        {
            Debug.Log(whale.transform.position.z);
            testPM.Move2D(1);
            yield return null;
        }
        
        Assert.That(System.Math.Abs(whale.transform.position.z) > initZ);
    }
}
