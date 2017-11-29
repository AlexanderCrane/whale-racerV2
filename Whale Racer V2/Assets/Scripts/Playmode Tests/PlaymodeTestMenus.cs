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
        UnityEngine.Object.DestroyImmediate(GameObject.Find("GameController"));
    }
    /// <summary>
    /// Teardown. Destroy the DontDestroyOnLoad lobby manager generated when MPLobby is loaded.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        UnityEngine.Object.DestroyImmediate(GameObject.Find("LobbyManager"));
    }

    /// <summary>
    /// Test that SelectedMap and LoadMap correctly load a map from the menu.
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// Test that the selected map is properly set as the scene for the lobby manager to load once everybody is ready.
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// Test that the endgame scene reloads the main menu.
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator TestReloadMainMenu()
    {
        yield return null;
        SceneManager.LoadScene("Aduloo");
        yield return null;

        GameObject.FindObjectOfType<GameManager>().spFinishTimes.Add("0:01");
        GameObject.FindObjectOfType<GameManager>().winner = 1;
        yield return null;
        SceneManager.LoadScene("EndGame");
        yield return null;

        for (int i=0; i<500; i++)
        {
            yield return null;
        }
        Assert.That(SceneManager.GetActiveScene().name == "MainMenu");
    }
}
