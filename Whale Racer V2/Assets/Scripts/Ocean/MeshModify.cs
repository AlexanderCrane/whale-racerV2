/*
* Author: Austin Irvine
* Modified From: http://www.habrador.com/tutorials/unity-boat-tutorial/3-buoyancy/
* Date of Mod  : Nov. 3, 2017
* Brief : Used to create dynamic mesh for objects to float realistically on a moving water surface
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class used to create a dynamic mesh for objects to float realistically on a moving water surface
/// </summary>
public class MeshModify
{

    #region Information
    #endregion

    #region Private Variables
    private Transform objTransform;

    private Vector3[] objVerticeCoordinates; //obj slice being the vertices of current obj
    private int[] objTriangles;

    private float[] distanceToWater;
    #endregion

    #region Public Variables
    public Vector3[] objGlobalVertices;

    public List<TriangleType> underWaterTriangles = new List<TriangleType>();
    #endregion

    #region System Methods
    #endregion

    #region Custom Methods
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="obj"></param>
    public MeshModify(GameObject obj)
    {
        //transform
        objTransform = obj.transform;

        //arrays and lists init
        objVerticeCoordinates = obj.GetComponent<MeshFilter>().mesh.vertices;
        objTriangles = obj.GetComponent<MeshFilter>().mesh.triangles;

        //vertice global positions
        objGlobalVertices = new Vector3[objVerticeCoordinates.Length];
        //all distance to water
        distanceToWater = new float[objVerticeCoordinates.Length];
    }
    /// <summary>
    /// Finds distances to water objects and calculates distances.
    /// </summary>
    public void GenUnderwaterMesh()
    {
        //Clear all data from list
        underWaterTriangles.Clear();

        //find distances to water once
        for(int i = 0; i < objVerticeCoordinates.Length; i++)
        {
            Vector3 globalPosition = objTransform.TransformPoint(objVerticeCoordinates[i]);

            //save global positions and calculate distance to water from the water sim
            objGlobalVertices[i] = globalPosition;
            distanceToWater[i] = WaterConsole.current.DistanceToWater(globalPosition, Time.time);
        }

        //Add underwater mesh
        AddTriangles();
    }
    /// <summary>
    /// Creates water triangles.
    /// </summary>
    private void AddTriangles()
    {
        //List that will store the data we need to sort the vertices based on distance to water
        List<VertexData> vertexData = new List<VertexData>();

        //Add init data that will be replaced
        vertexData.Add(new VertexData());
        vertexData.Add(new VertexData());
        vertexData.Add(new VertexData());


        //Loop through all the triangles (3 vertices at a time = 1 triangle)
        int i = 0;
        while (i < objTriangles.Length)
        {
            //Loop through the 3 vertices
            for (int x = 0; x < 3; x++)
            {
                //Save the data we need
                vertexData[x].distance = distanceToWater[objTriangles[i]];

                vertexData[x].index = x;

                vertexData[x].globalVertexPos = objGlobalVertices[objTriangles[i]];

                i++;
            }


            //All vertices are above the water
            if (vertexData[0].distance > 0f && vertexData[1].distance > 0f && vertexData[2].distance > 0f)
            {
                continue;
            }


            //Create the triangles that are below the waterline

            //All vertices are underwater
            if (vertexData[0].distance < 0f && vertexData[1].distance < 0f && vertexData[2].distance < 0f)
            {
                Vector3 p1 = vertexData[0].globalVertexPos;
                Vector3 p2 = vertexData[1].globalVertexPos;
                Vector3 p3 = vertexData[2].globalVertexPos;

                //Save the triangle
                underWaterTriangles.Add(new TriangleType(p1, p2, p3));
            }
            //1 or 2 vertices are below the water
            else
            {
                //Sort the vertices
                vertexData.Sort((x, y) => x.distance.CompareTo(y.distance));

                vertexData.Reverse();

                //One vertice is above the water, the rest is below
                if (vertexData[0].distance > 0f && vertexData[1].distance < 0f && vertexData[2].distance < 0f)
                {
                    AddTrianglesOneAboveWater(vertexData);
                }
                //Two vertices are above the water, the other is below
                else if (vertexData[0].distance > 0f && vertexData[1].distance > 0f && vertexData[2].distance < 0f)
                {
                    AddTrianglesTwoAboveWater(vertexData);
                }
            }
        }
    }

    /// <summary>
    /// Build new triangles where one of the old vertices is above the water
    /// </summary>
    /// <param name="vertexData"></param>
    private void AddTrianglesOneAboveWater(List<VertexData> vertexData)
    {
        //H is always at position 0
        Vector3 H = vertexData[0].globalVertexPos;

        //Left of H is M
        //Right of H is L

        //Find the index of M
        int M_index = vertexData[0].index - 1;
        if (M_index < 0)
        {
            M_index = 2;
        }

        //We also need the heights to water
        float h_H = vertexData[0].distance;
        float h_M = 0f;
        float h_L = 0f;

        Vector3 M = Vector3.zero;
        Vector3 L = Vector3.zero;

        //This means M is at position 1 in the List
        if (vertexData[1].index == M_index)
        {
            M = vertexData[1].globalVertexPos;
            L = vertexData[2].globalVertexPos;

            h_M = vertexData[1].distance;
            h_L = vertexData[2].distance;
        }
        else
        {
            M = vertexData[2].globalVertexPos;
            L = vertexData[1].globalVertexPos;

            h_M = vertexData[2].distance;
            h_L = vertexData[1].distance;
        }


        //Now we can calculate where we should cut the triangle to form 2 new triangles
        //because the resulting area will always form a square

        //Point I_M
        Vector3 MH = H - M;

        float t_M = -h_M / (h_H - h_M);

        Vector3 MI_M = t_M * MH;

        Vector3 I_M = MI_M + M;


        //Point I_L
        Vector3 LH = H - L;

        float t_L = -h_L / (h_H - h_L);

        Vector3 LI_L = t_L * LH;

        Vector3 I_L = LI_L + L;


        //Save the data, such as normal, area, etc      
        //2 triangles below the water  
        underWaterTriangles.Add(new TriangleType(M, I_M, I_L));
        underWaterTriangles.Add(new TriangleType(M, I_L, L));
    }

    /// <summary>
    /// Build  new triangles where two of the old vertices are above the water
    /// </summary>
    /// <param name="vertexData"></param>
    private void AddTrianglesTwoAboveWater(List<VertexData> vertexData)
    {
        //H and M are above the water
        //H is after the vertice that's below water, which is L
        //So we know which one is L because it is last in the sorted list
        Vector3 L = vertexData[2].globalVertexPos;

        //Find the index of H
        int H_index = vertexData[2].index + 1;
        if (H_index > 2)
        {
            H_index = 0;
        }


        //We also need the heights to water
        float h_L = vertexData[2].distance;
        float h_H = 0f;
        float h_M = 0f;

        Vector3 H = Vector3.zero;
        Vector3 M = Vector3.zero;

        //This means that H is at position 1 in the list
        if (vertexData[1].index == H_index)
        {
            H = vertexData[1].globalVertexPos;
            M = vertexData[0].globalVertexPos;

            h_H = vertexData[1].distance;
            h_M = vertexData[0].distance;
        }
        else
        {
            H = vertexData[0].globalVertexPos;
            M = vertexData[1].globalVertexPos;

            h_H = vertexData[0].distance;
            h_M = vertexData[1].distance;
        }


        //Now we can find where to cut the triangle

        //Point J_M
        Vector3 LM = M - L;

        float t_M = -h_L / (h_M - h_L);

        Vector3 LJ_M = t_M * LM;

        Vector3 J_M = LJ_M + L;


        //Point J_H
        Vector3 LH = H - L;

        float t_H = -h_L / (h_H - h_L);

        Vector3 LJ_H = t_H * LH;

        Vector3 J_H = LJ_H + L;


        //Save the data, such as normal, area, etc
        //1 triangle below the water
        underWaterTriangles.Add(new TriangleType(L, J_H, J_M));
    }

  
    /// <summary>
    /// Helper class to store triangle data so we can sort the distances
    /// </summary>
    private class VertexData
    {
        //The distance to water from this vertex
        public float distance;
        //An index so we can form clockwise triangles
        public int index;
        //The global Vector3 position of the vertex
        public Vector3 globalVertexPos;
    }

   /// <summary>
   /// Displays the underwater mesh.
   /// </summary>
   /// <param name="mesh">The target mesh</param>
   /// <param name="name">The mesh's name</param>
   /// <param name="triangesData">The list of water triangles</param>
    public void DisplayMesh(Mesh mesh, string name, List<TriangleType> triangesData)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        //Build the mesh
        for (int i = 0; i < triangesData.Count; i++)
        {
            //From global coordinates to local coordinates
            Vector3 v1 = objTransform.InverseTransformPoint(triangesData[i].v1);
            Vector3 v2 = objTransform.InverseTransformPoint(triangesData[i].v2);
            Vector3 v3 = objTransform.InverseTransformPoint(triangesData[i].v3);

            vertices.Add(v1);
            triangles.Add(vertices.Count - 1);

            vertices.Add(v2);
            triangles.Add(vertices.Count - 1);

            vertices.Add(v3);
            triangles.Add(vertices.Count - 1);
        }

        //Remove the old mesh
        mesh.Clear();

        //Give it a name
        mesh.name = name;

        //Add the new vertices and triangles
        mesh.vertices = vertices.ToArray();

        mesh.triangles = triangles.ToArray();

        mesh.RecalculateBounds();
    }
    #endregion
}
