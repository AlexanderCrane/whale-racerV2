using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPBlowhole : MonoBehaviour {
    private string blowholeButton = "Blowhole";
    public GameObject blowholeVisual;
	void Awake()
    {
        if (!GameManager.gmInst.isMP)
        {
            if (gameObject.GetComponent<PlayerMovement>().splitscreenPlayer2)
            {
                blowholeButton = "p2Blowhole";
            }
        }
    }
	// Update is called once per frame
	void Update () {
		if (Input.GetButton(blowholeButton))
        {
            Splash(gameObject.transform.position, gameObject.transform.rotation);
        }
	}

    public void Splash(Vector3 position, Quaternion rotation)
    {
        //instantiate the splash effect asset at point of contact
        GameObject spray = Instantiate(blowholeVisual, position, rotation);

        AnimateSplash(spray);
    }

    void AnimateSplash(GameObject splash)
    {
        splash.transform.position += (1 * Vector3.up);
        //destroy splash in 3 seconds
        Destroy(splash, 3);
    }
}
