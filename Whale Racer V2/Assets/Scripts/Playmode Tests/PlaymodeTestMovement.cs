using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections;

public class TestMovement : IPrebuildSetup {
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
        for (int i =0; i<100; i++)
        {
            testPM.Dive(true); //calling Dive while above water decreases whale height
            yield return null;
        }
        float postDiveY = whale.transform.position.y;
        Assert.That(postDiveY< initY);
        for (int i = 0; i < 150; i++)
        {
            yield return null;
        }
        Assert.That(whale.transform.position.y > postDiveY);

    }
    [UnityTest]
    public IEnumerator Test2DMoveModifiesPosition()
    {
        yield return null;
        RealSetup();
        float initZ = System.Math.Abs(whale.transform.position.z); //atm whale is pointing in the -z direction by default so use absolute values
        for (int i = 0; i < 100; i++) //move forward for 50 frames
        {
            testPM.Move2D(1);
            yield return null;
        }
        float postMoveFowardZ = System.Math.Abs(whale.transform.position.z);
        Assert.That(postMoveFowardZ > initZ); //pass if forward movement has occurred
        for (int i = 0; i < 100; i++) //move backward for 50 frames
        {
            testPM.Move2D(-1);
            yield return null;
        }
        Assert.That(System.Math.Abs(whale.transform.position.z) < postMoveFowardZ); //pass if backward movement has occurred
    }
    [UnityTest]
    public IEnumerator TestTurnModifiesRotation()
    {
        yield return null;
        RealSetup();
        float initRotation = whale.transform.rotation.eulerAngles.y;
        for (int i = 0; i<20; i++)
        {
            testPM.Turn(0, 1);
            yield return null;
        } //turn right for 20 frames, verify that our y rotation increases
        float postTurnRotation = whale.transform.rotation.eulerAngles.y;
        Assert.That(postTurnRotation > initRotation);
        Debug.Log("postturn: " + postTurnRotation);
        //don't turn left for quite as long as we turn right
        //if we turn too far left and get into a y angle which is shown as negative in the editor
        //the .y property of the transform returns it as the equivalent positive angle which will break this test
        //ie -15degrees = 345degrees
        //if you changed turn speed and this test broke that's probably why
        for (int i =0; i<10; i++) 
        {
            testPM.Turn(0, -1);
            yield return null;
        } //turn left for 10 frames, verify that y rotation decreases
        Debug.Log(whale.transform.rotation.eulerAngles.y);
        Assert.That(whale.transform.rotation.eulerAngles.y < postTurnRotation);
    }
    [UnityTest]
    public IEnumerator TestSpiralModifiesYRotationOnSurface()
    {
        yield return null;
        RealSetup();
        float initY = whale.transform.rotation.eulerAngles.y;
        for (int i = 0; i < 500; i++)
        {
            testPM.Move2D(1);
            testPM.Spiral(1);
            yield return null;
        }
        Assert.That(whale.transform.rotation.eulerAngles.y > initY);
    }

    [UnityTest]
    public IEnumerator TestSpiralModifiesZRotationUnderWater()
    {
        yield return null;
        RealSetup();
        float initZ = whale.transform.rotation.eulerAngles.z;
        for (int i = 0; i < 500; i++)
        {
            testPM.Dive(true);
            testPM.Move2D(1);
            testPM.Spiral(1);
            yield return null;
        }
        Assert.That(whale.transform.rotation.eulerAngles.z > initZ);
    }
    [UnityTest]
    public IEnumerator TestSpeedupPowerupIncreasesSpeed()
    {
        yield return null;
        RealSetup();
        testPM.SpeedupPowerup(5, new AudioClip());
        Assert.That(testPM.maxForwardSpeed == testPM.baseMaxForward * 1.5);
        Assert.That(testPM.maxBackwardSpeed == testPM.baseMaxBackward * 1.5);
        Assert.That(testPM.turnSpeed == testPM.baseTurnSpeed * 1.5);
    }
    [UnityTest]
    public IEnumerator TestBounceBackBouncesBack()
    {
        yield return null;
        RealSetup();
        float initPos = whale.transform.position.z;
        Vector3 bouncebackVector = new Vector3(0, 0, 100);
        testPM.BounceBack(bouncebackVector, new AudioClip());
        yield return null;
        Assert.That(whale.transform.position.z > initPos);
    }

    [UnityTest]
    public IEnumerator TestSlowdown()
    {
        yield return null;
        RealSetup();        
        testPM.slowDown();
        yield return null;
        Assert.That(testPM.maxForwardSpeed == testPM.baseMaxForward * .2);
        Assert.That(testPM.maxBackwardSpeed == testPM.baseMaxBackward * .2);
    }
}
