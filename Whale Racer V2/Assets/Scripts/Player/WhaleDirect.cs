/*
 * Author: Austin Irvine
 * File  : WhaleDirect.cs
 * Date  : November 24, 2017
 * Mod From: MouseLook.cs from Unity Stan. Assets
 * Brief: point whale in any direction and go under water in 3d
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
/// <summary>
///Whale Direction Script Controls 3D Movement & Direction
/// </summary>
[Serializable]
public class WhaleDirect {

    #region Private & Private Serialized variables and properties
    private Quaternion m_playerTargetRotation;
    private Quaternion m_camTargetRotation;
    private bool m_cursorIsLocked = true;

    [SerializeField] private float xSensitivity = 2.0f;
    [SerializeField] private float ySensitivity = 2.0f;

    [SerializeField] private bool clampVertRotation = true;

    [SerializeField] private float minXAngle = -90.0f;
    [SerializeField] private float maxXAngle = 90.0f;
    
    [SerializeField] private float smoothingFactor = 4.0f;
    [SerializeField] private bool cursorLock = true;
    [SerializeField] private bool smoothingEnabled = true;
    #endregion
    //NA
    #region Public/Protected variables and properties
    #endregion
    //NA
    #region System Methods & Initialization
    #endregion
    //Methods For Directing Whale
    #region Custom Methods
    /// <summary>
    /// Init. Awakens class with initial player and camera transforms
    /// </summary>
    public void Init(Transform player, Transform cam)
    {
        m_camTargetRotation = cam.localRotation;
        m_playerTargetRotation = player.localRotation;
    }
    /// <summary>
    /// Rotates And Look. Rotates player and camera
    /// </summary>
    public void RotateAndLook(Transform player, Transform cam)
    {
        float yRotation = CrossPlatformInputManager.GetAxis("Mouse X") * xSensitivity;
        float xRotation = CrossPlatformInputManager.GetAxis("Mouse Y") * ySensitivity;

        m_camTargetRotation *= Quaternion.Euler(0.0f, 0.0f, 0.0f);
        m_playerTargetRotation *= Quaternion.Euler(xRotation, yRotation, 0.0f);

        if(clampVertRotation)
        {
            m_camTargetRotation = ClampRotationOnX(m_camTargetRotation);
        }

        if(smoothingEnabled)
        {
            player.localRotation = Quaternion.Slerp(player.rotation, m_playerTargetRotation,
                smoothingFactor * Time.deltaTime);      

            //cam.localRotation = Quaternion.Slerp(cam.localRotation, m_camTargetRotation,
            //    smoothingFactor * Time.deltaTime);
        }
        else
        {
            player.localRotation = m_playerTargetRotation;
            //cam.localRotation = m_camTargetRotation;
        }
    }
    /// <summary>
    /// Clamp Rotation on X Axis. Returns quaternion within bounds for player
    /// </summary>
    Quaternion ClampRotationOnX(Quaternion q)
    {
        //x,y,z,w
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.z;
        q.w = 1.0f;

        float xAngle = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        xAngle = Mathf.Clamp(xAngle, minXAngle, maxXAngle);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * xAngle);

        return q;
    }
    /// <summary>
    /// Set Lock On Cursor. Locks or Unlocks Cursor and Controls
    /// </summary>
    public void SetLockOnCursor(bool key)
    {
        cursorLock = key;

        if(!cursorLock)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    /// <summary>
    /// Update Lock. Unlocks or Locks Controls, Detailed.
    /// </summary>
    public void UpdateLock()
    {
        //properly lock the cursor
        if (cursorLock)
        {
            if(Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if(Input.GetKeyUp(KeyCode.P))
            {
                m_cursorIsLocked = true;
            }

            if(m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if(!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
    #endregion

}
