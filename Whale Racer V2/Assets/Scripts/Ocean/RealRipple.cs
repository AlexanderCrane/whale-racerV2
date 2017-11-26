/*
 * Author: Austin Irvine
 * File: RealRipple.cs
 * Date: November 15, 2017
 * Purpose: Send scene information of objects that should produce waves
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Water
{
    [RequireComponent(typeof(WaterBase))]
    [ExecuteInEditMode]
    public class RealRipple : MonoBehaviour
    {
        public Vector3 waterHeight1;
        //[SerializeField] private List<GameObject> objectsInWater;
        [SerializeField] private List<GameObject> whales;
        //[SerializeField] private GameObject whale2;
        //[SerializeField] private GameObject whale3;
        //[SerializeField] private GameObject whale4;
        //[SerializeField] private GameObject whale5;
        [SerializeField] private List<float> speeds;
        //private float speed2;
        //private float speed3;
        //private float speed4;
        //private float speed5;
        private WaterBase m_WaterBase;


        private void Start()
        {
            for(int i = 0; i < whales.Count; i++)
            {
                speeds[i] = whales[i].GetComponent<PlayerMovement>().whaleSpeed;
            }
            m_WaterBase = (WaterBase)gameObject.GetComponent(typeof(WaterBase));
            waterHeight1 = new Vector3(0.0f, 0.0f, 0.0f);
        }


        public void Update()
        {
            //speed = whale.GetComponent<PlayerMovement>().whaleSpeed;
            if (!m_WaterBase)
            {
                m_WaterBase = (WaterBase)gameObject.GetComponent(typeof(WaterBase));
            }

            if (m_WaterBase.sharedMaterial)
            {
                for (int i = 0; i < whales.Count; i++)
                {
                    speeds[i] = whales[i].GetComponent<PlayerMovement>().whaleSpeed;
                }

                m_WaterBase.sharedMaterial.SetVector("_ObjectDisp1", whales[0].transform.position);
                if (speeds[0] == 0)
                {
                    m_WaterBase.sharedMaterial.SetFloat("_ObjectSpeed1", 0.2f);
                }
                else if (speeds[0] < 700)
                {
                    m_WaterBase.sharedMaterial.SetFloat("_ObjectSpeed1", .5f);
                }
                else
                {
                    m_WaterBase.sharedMaterial.SetFloat("_ObjectSpeed1", .85f);
                }

                m_WaterBase.sharedMaterial.SetVector("_ObjectDisp2", whales[1].transform.position);
                if (speeds[1] == 0)
                {
                    m_WaterBase.sharedMaterial.SetFloat("_ObjectSpeed2", 0.2f);
                }
                else if(speeds[1] < 700)
                {
                    m_WaterBase.sharedMaterial.SetFloat("_ObjectSpeed2", .5f);
                }
                else
                {
                    m_WaterBase.sharedMaterial.SetFloat("_ObjectSpeed2", .85f);
                }
            }
        }
    }
}
