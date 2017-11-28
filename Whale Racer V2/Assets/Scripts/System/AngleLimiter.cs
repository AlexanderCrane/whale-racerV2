/*
* Author: Austin Irvine
* Date of Mod  : Nov. 5, 2017
* Brief : Limits Buoyant Objects From Moving Too Radically
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Script to limit movement of buoyant objects.
/// </summary>
public class AngleLimiter : MonoBehaviour {

    #region Private Variables & Properties
    //[SerializeField] Transform target;
    [SerializeField] float rotateSpeed;
    private Rigidbody rb;
    private Quaternion lookDirection;
    //private Vector3 correctDirection;
    #endregion

    #region Public Variables & Properties
    #endregion

    #region System Methods
        /// <summary>
        /// Start method. Gets the containing object's rigidbody.
        /// </summary>
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }
    /// <summary>
    /// FixedUpdate method. Calls ReStabilize if buoyant object is out of alignment.
    /// </summary>
    private void FixedUpdate ()
    {
        //RaycastHit hitFloor;
        //hitDown;
        Ray downRay = new Ray(transform.position, transform.TransformDirection(-Vector3.forward));
        Ray down = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.forward) * 40);
        Debug.DrawRay(transform.position, Vector3.down * 40);
        //Physics.Raycast(downRay, out hitFloor, 10.0f);
        //Physics.Raycast(down, out hitDown, 10.0f);

        //
        //Difference between two vectors
        //
        Vector3 newVector = Vector3.Cross(transform.TransformDirection(-Vector3.forward), Vector3.down);
        float AngleDifference = Mathf.Abs(Vector3.Angle(transform.TransformDirection(-Vector3.forward), Vector3.down));
        //Debug.Log(AngleDifference);
        if (AngleDifference > 30)
        {
            //Debug.Log("Send it back");
            ReStabilize(AngleDifference);
        }

        /*if (hitFloor.collider.tag == "Floor")
        {
            Debug.Log("Hit the bottom");
        }*/
    }
    /// <summary>
    /// Realigns a buoyant object by Slerping it back.
    /// </summary>
    /// <param name="angle"></param>
    private void ReStabilize(float angle)
    {
        lookDirection = Quaternion.LookRotation(-Vector3.down);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, Time.deltaTime * rotateSpeed);
    }
    #endregion
}
