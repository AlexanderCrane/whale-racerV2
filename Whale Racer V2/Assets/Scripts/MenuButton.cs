using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour {
    public bool isPlayButton;
    public bool isQuitButton;
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
    private void OnMouseEnter()
    {
        GetComponent<Renderer>().material.color = Color.yellow;

    }
    private void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = Color.white;

    }
}
