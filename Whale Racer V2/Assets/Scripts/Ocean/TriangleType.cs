/*
 * Author: Austin Irvine
 * Modified From: http://www.habrador.com/tutorials/unity-boat-tutorial/3-buoyancy/
 * Date of Mod  : Nov. 3, 2017
 * Brief : Stores Triangle Mesh Data
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct TriangleType
{

    #region Information
    #endregion

    #region Private Variables
    #endregion

    #region Public Variables
    public Vector3 v1;
    public Vector3 v2;
    public Vector3 v3;

    //triangle center
    public Vector3 center;

    //distance to the surface
    public float distanceToSurface;

    //triangle normal direction
    public Vector3 normal;

    //area of triangle
    public float area;
    #endregion

    #region System Methods

    #endregion

    #region Custom Methods
    public TriangleType(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;

        //center
        this.center = (v1 + v2 + v3) / 3f;

        //distance
        this.distanceToSurface = Mathf.Abs(WaterConsole.current.DistanceToWater(this.center, Time.time));

        //normal
        this.normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;

        //area of triangle
        float a = Vector3.Distance(v1, v2);
        float c = Vector3.Distance(v3, v1);

        this.area = (a * c * Mathf.Sin(Vector3.Angle(v2 - v1, v3 - v1) * Mathf.Deg2Rad)) / 2f;
    }
    #endregion
}