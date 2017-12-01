using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Class which handles input to trigger the whale's blowhole.
/// </summary>
public class Blowhole : MonoBehaviour {
    private string blowholeButton = "Blowhole";
    public GameObject blowholeVisual;

    public AudioClip previousClip;
    public AudioClip blowClip;
    public AudioSource aSource;
    /// <summary>
    /// Awake method. Sets the blowhole control to an alternate value if this player is splitscreen p2.
    /// </summary>
	void Awake()
    {
        if (gameObject.GetComponent<PlayerMovement>().splitscreenPlayer2)
        {
            blowholeButton = "p2Blowhole";
        }
    }
    /// <summary>
    /// Update method. Triggers splash if the blowhole button is pressed. 
    /// </summary>
    void Update () {
		if (Input.GetButton(blowholeButton))
        {
            Splash(gameObject.transform.position, gameObject.transform.rotation);
        }
	}
    /// <summary>
    /// Similar to whale fin splash method - instantiates a splash sprite and calls AnimateSplash.
    /// </summary>
    /// <param name="position">The position to splash at</param>
    /// <param name="rotation">The rotation of the whale</param>
    public void Splash(Vector3 position, Quaternion rotation)
    {
        //instantiate the splash effect asset at point of contact
        GameObject spray = Instantiate(blowholeVisual, position, rotation);
        if(aSource != null && blowClip != null && previousClip == null)
        {
            previousClip = aSource.clip;
            aSource.clip = blowClip;
            AnimateSplash(spray);
            aSource.Play();
            StartCoroutine("DelayRoutine");
        }
    }
    /// <summary>
    /// Moves the input splash sprite up and despawns it.
    /// </summary>
    private IEnumerator DelayRoutine()
    {
        yield return new WaitForSeconds(0.40f);
        aSource.Stop();
        aSource.clip = previousClip;
        aSource.Play();
        previousClip = null;
        StopAllCoroutines();
        yield return false;
    }
    /// <summary>
    /// Moves the input splash sprite up and despawns it.
    /// </summary>
    /// <param name="splash">The object to move up</param>
    void AnimateSplash(GameObject splash)
    {
        splash.transform.position += (1 * Vector3.up);
        //destroy splash in 3 seconds
        Destroy(splash, 3);
    }
}
