using System.Collections;		
 using System.Collections.Generic;		
 using UnityEngine;		
/// <summary>
/// Class used to rotate powerups.
/// </summary>
 public class Rotator : MonoBehaviour
{		
 	// Update is called once per frame		
    /// <summary>
    /// Rotates the gameobject.
    /// </summary>
 	void Update()
    {
        
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
        
          }		
 }