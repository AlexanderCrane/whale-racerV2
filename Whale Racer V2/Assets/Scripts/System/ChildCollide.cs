using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container class to be used to hold a specifiable parent object for a gameobject.
/// Lets us ensure that we can always get the parent whale from any component of the whale rig.
/// Needed to treat all parts of the whale as the whale for collision detection.
/// </summary>
public class ChildCollide : MonoBehaviour {
    public GameObject collisionParent; 
	
}
