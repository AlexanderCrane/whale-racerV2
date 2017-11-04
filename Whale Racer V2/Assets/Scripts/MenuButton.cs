using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Outdated menu button script. Handles Play and Quit buttons in the old main menu
/// </summary>
public class MenuButton : MonoBehaviour {
    public bool isPlayButton;
    public bool isQuitButton;
    /// <summary>
    /// OnMouseUp method for menu buttons. Changes to main map scene if play button, quits if quit button.
    /// </summary>
    private void OnMouseUp()
    {
        if (isPlayButton)
        {
            SceneManager.LoadScene(1);
        }
        else if (isQuitButton)
        {
            Application.Quit();
        }
    }
    /// <summary>
    /// OnMouseEnter method for menu buttons. Highlights them on hover.
    /// </summary>
    private void OnMouseEnter()
    {
        GetComponent<Renderer>().material.color = Color.yellow;

    }
    /// <summary>
    /// OnMouseExit method for menu buttons. Removes hover highlight.
    /// </summary>
    private void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = Color.white;

    }
}
