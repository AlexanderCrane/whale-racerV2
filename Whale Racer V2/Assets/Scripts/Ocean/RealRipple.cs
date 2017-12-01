/*
 * Author: Austin Irvine
 * File: RealRipple.cs
 * Date: November 15, 2017
 * Purpose: Send scene information of objects that should produce waves
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace UnityStandardAssets.Water
{
    /// <summary>
    /// Script to send scene information about objects that should produce ripples.
    /// </summary>
    [RequireComponent(typeof(WaterBase))]
    [ExecuteInEditMode]

    public class RealRipple : MonoBehaviour
    {
        public Vector3 waterHeight1;
        [SerializeField] private List<GameObject> whales;
        [SerializeField] private List<float> speeds;
        private WaterBase m_WaterBase;

        /// <summary>
        /// Start method. Gets whale speeds and water height.
        /// </summary>
        private void Start()
        {
            //CheckForWhales();

            for (int i = 0; i < whales.Count; i++)
            {
                if (whales[i] != null) { 
                    speeds[i] = whales[i].GetComponent<PlayerMovement>().whaleSpeed;
                }
            }
            m_WaterBase = (WaterBase)gameObject.GetComponent(typeof(WaterBase));
            waterHeight1 = new Vector3(0.0f, 0.0f, 0.0f);
        }
        /// <summary>
        /// Adds whales in scene to displace the water surface
        /// </summary>
        private void CheckForWhales()
        {
            Debug.Log("Populating whales");
            foreach (GameObject whale in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (whale.name.StartsWith("MP Whale"))
                {
                    whales.Add(whale);
                    speeds.Add(0);
                }
            }
        }
        /// <summary>
        /// Populates whales in multiplayer and creates ripples around whales.
        /// </summary>
        public void Update()
        {
            if (SceneManager.GetActiveScene().name == "MPLobby")
            {
                return;
            }
            if (GameManager.gmInst.isMP && whales.Count == 0)
            {
                CheckForWhales();
            }
            //speed = whale.GetComponent<PlayerMovement>().whaleSpeed;
            if (!m_WaterBase)
            {
                m_WaterBase = (WaterBase)gameObject.GetComponent(typeof(WaterBase));
            }

            if (m_WaterBase.sharedMaterial)
            {
                for (int i = 0; i < whales.Count; i++)
                {
                    if (whales[i] != null)
                    {
                        speeds[i] = whales[i].GetComponent<PlayerMovement>().whaleSpeed;
                    }
                }

                if (whales.Count > 0 && whales[0] != null)
                {
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
                }

                if (whales.Count > 1 && whales[1] != null)
                {
                    m_WaterBase.sharedMaterial.SetVector("_ObjectDisp2", whales[1].transform.position);
                    if (speeds[1] == 0)
                    {
                        m_WaterBase.sharedMaterial.SetFloat("_ObjectSpeed2", 0.2f);
                    }
                    else if (speeds[1] < 700)
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
}
