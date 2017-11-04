using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///<summary>
///Script to generate splash sprites as the player moves.
///</summary>
public class PlayerSplash : MonoBehaviour {


    public GameObject SplashVisual;
    ///<summary>
    ///OnTriggerEnter method. Checks for collision between whale's splash colliders and water, then triggers splash.
    ///<param name="other">The object colliding with the whale.</param>
    ///</summary>
    void OnTriggerEnter (Collider other) {
		if (other.CompareTag("SplashCollider"))
        {
            Rigidbody colliderRB = other.GetComponent<Rigidbody>();

            Splash(colliderRB.position, colliderRB.rotation);
        }
	}
    ///<summary>
    ///Plays splash animation at given position.
    ///<param name="position">The position to create the splash at.</param>
    ///<param name="rotation">The initial rotation of the splash.</param>
    ///</summary>
    void Splash(Vector3 position, Quaternion rotation)
    {
        //instantiate the splash effect asset at point of contact
        GameObject spray = Instantiate(SplashVisual, position, rotation);

        AnimateSplash(spray);
    }
    ///<summary>
    /// Moves splash sprite up from its point of creation, then destroys it once it expires.
    /// <param name="splash">The splash sprite to be animated.</param>
    ///</summary>
    void AnimateSplash(GameObject splash)
    {
        splash.transform.position += (1 * Vector3.up);
        //destroy splash in 3 seconds
        Destroy(splash, 3);
    }
}
