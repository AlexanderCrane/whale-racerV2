using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections;
using UnityEngine.AI;
using System.Linq;
/// <summary>
/// Tests for aspects of game setup related to splitscreen.
/// </summary>
public class TestSplitscreen : IPrebuildSetup {
    private GameObject whale;
    private PlayerMovement testPM;
    private Transform testTransform;

    /// <summary>
    /// Loads Aduloo.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        LoadMap.passSplitscreen = true;
        SceneManager.LoadScene("Aduloo");
    }
    /// <summary>
    /// Teardown. Destroys DontDestroyOnLoad GameControllers left behind when Aduloo is loaded.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        UnityEngine.Object.Destroy(GameObject.FindGameObjectWithTag("GameController"));
    }
    /// <summary>
    /// Function run after running the desired testing scene for one frame so that things can be initialized.
    /// Doing setup this way lets us use the actual play scenes for testing to save time on mocking.
    /// This is a hack.
    /// </summary>
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
    /// <summary>
    /// Test that the NavMeshAgent AI is disabled and the 'AI Whale' is controlled by a player if splitscreen is enabled.
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator TestSplitscreenAIDisabled()
    {
        yield return null;
        GameObject ai = GameObject.Find("AI_Whale");
        Assert.That(ai.GetComponent<NavMeshAgent>().enabled == false);
        Assert.That(ai.GetComponent<PlayerMovement>().enabled == true);
    }
    /// <summary>
    /// Test that both cameras are enabled and displayed if splitscreen is enabled.
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator TestSplitscreenCamerasSplit()
    {
        yield return null;
        //for some reason ive decided to do all the camera finding stuff with linq 
        //just for practice i guess
        Camera[] cams = GameObject.FindGameObjectsWithTag("PlayerCamera").Select(obj => obj.GetComponent<Camera>())
            .Where(obj=>obj.enabled == true)
            .Where(obj=>obj.rect.height !=0 && obj.rect.width !=0)
            .ToArray(); //get all enabled player cameras which are being displayed on the screen
        Assert.That(cams.Length == 2); // should be two of them
    }
}
