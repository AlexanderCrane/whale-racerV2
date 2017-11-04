using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterConsole : MonoBehaviour
{

    #region Information
    #endregion

    #region Private Variables
    [SerializeField] private float waterHeight = -1.0f;
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
    private void Start()
    {
        current = this;
    }
    #endregion

    #region Custom Methods
    public float GetWaveYPos(Vector3 position, float timeSinceStart)
    {
        return waterHeight;
    }

    public float DistanceToWater(Vector3 position, float timeSinceStart)
    {
        float waterHeight = GetWaveYPos(position, timeSinceStart);

        float distanceToWater = position.y - waterHeight;

        return distanceToWater;
    }
    #endregion
}
