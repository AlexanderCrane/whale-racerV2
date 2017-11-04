/*
**Author: Austin Irvine
**Dated : November 2, 2017
**Brief   : Creates a buoyancy effect on an object
*/

using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class SimpleBoyancy : MonoBehaviour
{
    #region Information & Physics
    /*
        source: http://wiki.ubc.ca/Buoyancy,_Pressure,_Bernoulli%27s_Equation
        ref   : http://www.habrador.com/tutorials/unity-boat-tutorial/3-buoyancy/

        Bernoulli's Equation:
        F = V(pL)g  OR 
        Boyant Force = Volume(m^3) * Density of Liquid (kg/m^3) * Force of gravity(N/kg)

        What to do with an object:
        Calculate Volume of the submerged section of object
            ex: V = h(pi)(r^2) - cylinder
        Calculate Force of gravity from mass  (F=mg)
        Find Density of Water
            p=1000kg/m^3
            mass of water
                m = pV = (1000kg/m^3) * (0.032m)*(pi)*(r^2) = 32*pi*r^2kg


        Idea:
        Use Archimedes Law and find displacement of mesh under and over water
        The mesh underwater will displace water pushing object up and down via gravity
    */
    #endregion

    #region Private Variables
    [SerializeField] private float forceB = 0.0f;
    [SerializeField] private float volume = 0.0f;
    [SerializeField] private float density = 1000.0f;
    [SerializeField] private float massOfWater = 0.0f;

    private Rigidbody baseRB;
    private MeshModify meshMod;
    private Mesh underwaterMesh;
    #endregion

    #region Public Variables
    public GameObject underwaterObj;
    #endregion

    #region System Methods
    private void Awake()
    {
        underwaterObj.AddComponent<MeshFilter>();
        underwaterObj.AddComponent<MeshRenderer>();
    }

    private void Start()
    {
        baseRB = gameObject.GetComponent<Rigidbody>();

        meshMod = new MeshModify(gameObject);

        underwaterMesh = underwaterObj.GetComponent<MeshFilter>().mesh;
    }

    private void Update()
    {
        meshMod.GenUnderwaterMesh();

        meshMod.DisplayMesh(underwaterMesh, "Underwater Mesh" ,meshMod.underWaterTriangles);
    }

    private void FixedUpdate()
    {
        //Physics
        if (meshMod.underWaterTriangles.Count > 0)
        {
            AddWaterForces();
        }
    }
    #endregion

    #region Custom Methods
    private void AddWaterForces()
    {
        //Get Surfaces Underwater
        List<TriangleType> underWaterTriangles = meshMod.underWaterTriangles;

        for (int i = 0; i < underWaterTriangles.Count; i++)
        {
            TriangleType tri = underWaterTriangles[i];

            Vector3 buoyancyForce = BuoyancyForce(density, tri);

            baseRB.AddForceAtPosition(buoyancyForce, tri.center);
        }
    }

    private Vector3 BuoyancyForce(float density, TriangleType tri)
    {
        Vector3 forceBuoy = density * Physics.gravity.y * tri.distanceToSurface * tri.area * tri.normal;

        forceBuoy.x = 0.0f;
        forceBuoy.z = 0.0f;

        return forceBuoy;
    }
    #endregion
}
