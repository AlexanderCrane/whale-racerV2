﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lerpCamera : MonoBehaviour
{

    [SerializeField] public GameObject target;

    [SerializeField] Vector3 defaultDistance = new Vector3(0f, 2f, -10f);

    [SerializeField] float distanceDamp = 10f;
    [SerializeField] float rotationalDamp = 10f;

    Transform myT;


    // Use this for initialization
    void Awake()
    {
        myT = transform;

    }

    // LateUpdate acts as Update but one frame late
    void LateUpdate()
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