/*
* Author: Austin Irvine
* Date of Mod  : Nov. 5, 2017
* Brief : Adds Underwater camera effects
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityStandardAssets.ImageEffects;

public class UnderWaterEffects : MonoBehaviour {

    #region Private Variables and Properties
    //[SerializeField] GameObject cameraFilter;
    private ColorCorrectionCurves cCC;
    private Blur blur;
    #endregion

    private void Start()
    {
        cCC = this.GetComponent<ColorCorrectionCurves>();
        blur = this.GetComponent<Blur>();
    }

    void Update () {
		if(this.transform.position.y < 0.0f)
        {
            ActivateFilters();
        }
        else
        {
            DeactivateFilters();
        }
	}

    private void ActivateFilters()
    {
        cCC.enabled = true;
        blur.enabled = true;
    }

    private void DeactivateFilters()
    {
        cCC.enabled = false;
        blur.enabled = false;
    }
}
