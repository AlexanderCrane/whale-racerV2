using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Water;
using UnityEngine.Networking;
/// <summary>
/// Class to customize ocean behavior.
/// </summary>
public class WaterConsole : NetworkBehaviour
{

    #region Information
    #endregion

    #region Private Variables
    [SerializeField] private float waterHeight = 0.0f;
    #endregion

    #region Public Variables
    public static WaterConsole current;
    public bool isFlowing;

    //wave height & speed
    public float scale = 0.1f;
    public float speed = 1.0f;

    //wave width
    public float waveDistance = 1.0f;

    //Noise parameters
    public float noiseStrength = 1.0f;
    public float noiseWalk = 1.0f;
    #endregion

    #region System Methods
    private void Awake()
    {
        current = this;
    }
    #endregion

    #region Custom Methods
    /// <summary>
    /// Returns the water's height.
    /// </summary>
    /// <param name="position">Unused</param>
    /// <param name="timeSinceStart">Unused</param>
    /// <returns></returns>
    public float GetWaveYPos(Vector3 position, float timeSinceStart)
    {
        return waterHeight;
    }
    /// <summary>
    /// Get a position's distance from the water.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="timeSinceStart">Time the ocean has been running.</param>
    /// <returns></returns>
    public float DistanceToWater(Vector3 position, float timeSinceStart)
    {
        float waterHeight = GetWaveYPos(position, timeSinceStart);

        float distanceToWater = position.y - waterHeight;

        return distanceToWater;
    }
    #endregion
}
