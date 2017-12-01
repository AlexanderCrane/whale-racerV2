using UnityEngine;
using System.Collections;
/// <summary>
/// class to destroy the parent gameObject after a specified time.
/// </summary>
public class DestroyTimed : MonoBehaviour {

	public float time = 5f;
    /// <summary>
    /// Destroys the host object after 5 seconds.
    /// </summary>
	void Start () {
		Destroy (gameObject, time);
	}
}
