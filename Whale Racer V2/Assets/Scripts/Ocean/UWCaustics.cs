using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UWCaustics : MonoBehaviour
{
    #region Public Fields and Properties
    public float FPS = 30.0f;
    public Texture[] caustics;

    #endregion

    #region Private Fields and Properties
    private new Renderer renderer;
    #endregion

    #region Standard Methods
    // Use this for initialization
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (caustics.Length >= 1 && caustics != null)
        {
            int causticIndex = (int)(Time.time * FPS) % caustics.Length;
            renderer.sharedMaterial.SetTexture("_EmissionMap", caustics[causticIndex]); 
        }

        Vector3 LightDir = new Vector3(10, 10, 5);
        var lightMatrix = Matrix4x4.TRS(
                                                new Vector3(1, 2, 10),
                                                Quaternion.LookRotation(LightDir,
                                                            new Vector3(LightDir.z, LightDir.x, LightDir.y)),
                                                Vector3.one);
        renderer.sharedMaterial.SetMatrix("_CausticsLightOrientation", lightMatrix);
    }
    #endregion
}
