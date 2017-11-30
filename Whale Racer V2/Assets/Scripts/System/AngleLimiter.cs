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
    private float yRotation = 0.0f;
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
        yRotation = this.transform.eulerAngles.y;
        PositionReset();
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

    /// <summary>
    /// Rotates player back to resting position
    /// </summary>
    public void PositionReset() //0 for x&z, 1 for x, 2 for z
    {
        Quaternion fromRotation = transform.rotation;
        Quaternion targetRotation;
        yRotation = this.transform.eulerAngles.y;
        
        //X & Z
        targetRotation = Quaternion.Euler(new Vector3(-90.0f, yRotation, 0.0f));

        float xAng = transform.localEulerAngles.x;
        float zAng = transform.localEulerAngles.z;
        if ((xAng > -70.0f || xAng < -110.0f) || (zAng > 20.0f || zAng < -20.0f))
        {
            this.transform.localRotation = Quaternion.Slerp(this.transform.rotation, targetRotation,
                5.50f * Time.deltaTime);
        }
    }
    #endregion
}
