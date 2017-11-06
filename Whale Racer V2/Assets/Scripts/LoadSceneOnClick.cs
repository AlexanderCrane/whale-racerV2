using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class LoadSceneOnClick : MonoBehaviour {
	public void loadByIndex(int sceneIndex)
	{
		SceneManager.LoadScene (sceneIndex); 
	}
    public void quitGame()
    {
        Application.Quit();
    }
}
