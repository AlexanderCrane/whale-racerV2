using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceNBreak : MonoBehaviour {

    [SerializeField] private Vector3 correctDirection;

	// Use this for initialization
	void Start () {
        correctDirection = transform.forward;
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Box collided with");
            PlayerMovement collidingPlayer = other.gameObject.GetComponent<PlayerMovement>();
            collidingPlayer.BounceBack(correctDirection);
            Destroy(this.gameObject);

        }
    }
}
