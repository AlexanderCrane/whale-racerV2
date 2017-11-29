using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Tests for the main menu.
/// </summary>
public class TestMenus : IPrebuildSetup
{
    private GameObject whale;
    private PlayerMovement testPM;
    private Transform testTransform;

    /// <summary>
    /// Loads MainMenu.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene("MainMenu");
    }
    [TearDown]
    public void Teardown()
    {
        UnityEngine.Object.DestroyImmediate(GameObject.Find("LobbyManager"));
    }
    public void RealSetup()
    {
    }

    [UnityTest]
    public IEnumerator TestLoadMap()
    {
        yield return null;
        GameObject mapSelectorButton = GameObject.Find("AdulooButton");
        SelectedMap mapSelector = mapSelectorButton.GetComponent<SelectedMap>(); //selectedmap stores map the user selects in map selection
        mapSelector.setMap(2); //the shipyard
        GameObject spStartButton = GameObject.Find("strtbtn");
        spStartButton.GetComponent<LoadMap>().loadSelected(); //loadmap loads it
        yield return null;
        Assert.That(SceneManager.GetActiveScene().name == "TheShipyard");
    }

    [UnityTest]
    public IEnumerator TestLoadMapMP()
    {
        yield return null;
        GameObject mapSelectorButton = GameObject.Find("AdulooButton");
        SelectedMap mapSelector = mapSelectorButton.GetComponent<SelectedMap>(); //selectedmap stores map the user selects in map selection
        mapSelector.setMap(2); //the shipyard
        GameObject spStartButton = GameObject.Find("strtbtn");
        spStartButton.GetComponent<LoadMap>().loadSelectedMP(); //loadmap loads it
        yield return null;
        Assert.That(SceneManager.GetActiveScene().name == "MPLobby");
        Assert.That(GameObject.FindObjectOfType<NetworkLobbyManager>().playScene == "TheShipyard_MP"); //don't need to test that unity's lobby manager loads playscene correctly on start
        //just verify it gets set
    }
}
