using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSplash : MonoBehaviour {

    // upon hitting splash collider

    public GameObject SplashVisual;
	void OnTriggerEnter (Collider other) {
		if (other.CompareTag("SplashCollider"))
        {
            Rigidbody colliderRB = other.GetComponent<Rigidbody>();

            Splash(colliderRB.position, colliderRB.rotation);
        }
	}
	void Splash(Vector3 position, Quaternion rotation)
    {
        //instantiate the splash effect asset at point of contact
        GameObject spray = Instantiate(SplashVisual, position, rotation);

        AnimateSplash(spray);
    }
    //move splash effect up from point of contact
    void AnimateSplash(GameObject splash)
    {
        splash.transform.position += (1 * Vector3.up);
        //destroy splash in 3 seconds
        Destroy(splash, 3);
    }
	// Update is called once per frame
	void Update () {
		
	}
}
