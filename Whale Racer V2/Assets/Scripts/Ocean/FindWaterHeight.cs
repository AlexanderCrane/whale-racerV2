using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindWaterHeight : MonoBehaviour {

    public float waterY = 0;

    private void FixedUpdate()
    {
        //int maskLayers = LayerMask.GetMask("Water");
        RaycastHit hitFloor;
        //hitDown;
        Ray downRay = new Ray(transform.position, transform.TransformDirection(-Vector3.forward));
        Ray down = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(transform.position, Vector3.down * 10);
        
        if(Physics.Raycast(down, out hitFloor, 10.0f))
        {
            if (hitFloor.collider.tag == "Ocean")
            {
                Debug.Log("Hit Water");
            }
            else
            {
                Debug.Log("hitting sumthin: " + hitFloor.collider.tag);
            }
        }
    }
}
