using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

/// <summary>
/// Alternate version of the Lerp Camera for multiplayer. Finds itself a target dynamically instead of being assigned one.
/// </summary>
public class MPLerpCamera : NetworkBehaviour
{

    public GameObject target = null;

    [SerializeField] Vector3 defaultDistance = new Vector3(0f, 2f, -10f);

    [SerializeField] float distanceDamp = 10f;
    [SerializeField] float rotationalDamp = 10f;

    Transform myT;

    /// <summary>
    /// Awake method. Stores the camera's transform.
    /// </summary>
    void Awake()
    {
        myT = transform;

    }

    /// <summary>
    /// Delayed Update method. If the lerp camera does not have a target, disables it.
    /// Otherwise functions as a lerp camera.
    /// </summary>
    void LateUpdate()
    {

        if (target == null)
        {
            this.gameObject.GetComponent<Camera>().enabled = false;
            return;
        }
        else
        {
            if (target.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                this.gameObject.GetComponent<Camera>().enabled = true;
            }
            else
            {
                return;
            }
        }

        if (target != null)
        {
            Transform targetTransform = target.transform;
            Vector3 toPos = targetTransform.position + (targetTransform.rotation * defaultDistance);
            Vector3 curPos = Vector3.Lerp(myT.position, toPos, distanceDamp * Time.deltaTime);
            myT.position = curPos;

            Quaternion toRot = Quaternion.LookRotation(targetTransform.position - myT.position, targetTransform.up);
            Quaternion curRot = Quaternion.Slerp(myT.rotation, toRot, rotationalDamp * Time.deltaTime);
            myT.rotation = curRot;
        }
    }
    /// <summary>
    /// Function to assist player manager in finding a camera in MP
    /// In MP, the player is spawned based on a prefab when all players load in. 
    /// The cameras are already in the scene and cannot be manually assigned to it.
    /// </summary>
    /// <param name="target"></param>
    public void setTarget(GameObject target)
    {
        this.target = target;
    }
    /// <summary>
    /// Checks if the camera has a target.
    /// </summary>
    /// <returns></returns>
    public bool hasTarget()
    {
        return !(this.target == null);
    }
}
