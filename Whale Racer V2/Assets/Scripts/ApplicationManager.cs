using UnityEngine;
using System.Collections;
/// <summary>
/// Contains methods for the UI to exit the application.
/// </summary>
public class ApplicationManager : MonoBehaviour {

    /// <summary>
    /// Closes the application.
    /// </summary>
    public void Quit () 
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}
}
