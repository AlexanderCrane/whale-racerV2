/*
* Author: Austin Irvine
* Date of Mod  : Nov. 5, 2017
* Brief : Adds Underwater camera effects
*/

using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using UnityEngine;
using UnityEngine.Audio;

public class UnderWaterEffects : MonoBehaviour {

    #region Private Variables and Properties
    //[SerializeField] GameObject cameraFilter;
    private ColorCorrectionCurves cCC;
    private Twirl twirl;
    private TiltShift TS;
    private GlobalFog GF;
    private SunShafts sS;
    private VignetteAndChromaticAberration vCA;

    public AudioMixer audioMixer;
    public DepthAudio depthAud;

    private float dFalloff = 0.70f;
    private float blurStart = 0.35f;
    private float abbStart = 0.2f;

    //private float saturation = 0.79f;
    //private float bSpread = 0.606f;
    private float yPosition;
    private bool activatedUnd = false;
    private bool activatedOv = false;
    #endregion

    private void Start()
    {
        cCC = this.GetComponent<ColorCorrectionCurves>();
        GF = this.GetComponent<GlobalFog>();
        twirl = this.GetComponent<Twirl>();
        TS = this.GetComponent<TiltShift>();
        //sS = this.GetComponent<SunShafts>();
        vCA = this.GetComponent<VignetteAndChromaticAberration>();
    }
    /// <summary>
    /// Adjust audio and camera effects based on position relative to water.
    /// </summary>
    void Update ()
    {
        yPosition = this.transform.position.y;
        if (depthAud != null)
        {
            depthAud.AdjustAudio(yPosition);
        }

        if (yPosition < 1.0f && yPosition > -0.5f)
        {
            if(activatedOv)
            {
                ToWaterTransitions(yPosition);
            }
            else
            {
                //sS.enabled = true;
                //sS.maxRadius = dFalloff;
                vCA.enabled = true;
                activatedOv = true;
            }          
        }
        else if(yPosition >= 2.0f && vCA.enabled != false)
        {
            if(vCA.blurSpread <= blurStart) //sS.maxRadius <= dFalloff && 
            {
                //sS.enabled = false;
                vCA.enabled = false;
            }
            else
            {
                //sS.maxRadius = Mathf.Lerp(sS.maxRadius, dFalloff / 5, .3f);
                vCA.blurSpread = Mathf.Lerp(vCA.blurSpread, blurStart, .1f);
            }
            activatedOv = false;
        }

        if (yPosition < 0.0f)
        {
            if(!activatedUnd)
            {
                ActivateFilters();
                ActivateSFX();
                activatedUnd = true;
                DeactivateOverFilters();
            }
            else
            {
                UpdateSFX();
            }
        }
        else
        {
            if (activatedUnd)
            {
                DeactivateFilters();
                DeactivateSFX();
                activatedUnd = false;
            }
        }
	}
    /// <summary>
    /// Activate the underwater camera filters.
    /// </summary>
    private void ActivateFilters()
    {
        GF.height = 1.0f;
        cCC.enabled = true;
        TS.enabled = true;
        //twirl.enabled = true;
        GF.enabled = true;
    }
    /// <summary>
    /// Deactivate the underwater camera filters.
    /// </summary>
    private void DeactivateFilters()
    {
        cCC.enabled = false;
        TS.enabled = false;
        //twirl.enabled = false;
        GF.enabled = false;
    }
    /// <summary>
    /// Update the camera fog filter.
    /// </summary>
    private void UpdateSFX()
    {
        GF.height = 4.0f * (-this.transform.position.y);
    }
    /// <summary>
    /// Begin playing the underwater background sound.
    /// </summary>
    private void ActivateSFX()
    {
        BGMController.bgmInst.ToggleUnderWaterSound();
    }
    /// <summary>
    /// Begin playing the above water background sound.
    /// </summary>
    private void DeactivateSFX()
    {
        BGMController.bgmInst.ToggleAboveWaterSound();
    }
    /// <summary>
    /// Initialize the vignette effect.
    /// </summary>
    /// <param name="yPos"></param>
    private void ToWaterTransitions(float yPos)
    {
        vCA.blurSpread = blurStart + ((2 - yPosition) / 2);
        /*if (yPos > 0.0f && yPos < 0.3f)
        {
            sS.enabled = true;
        }
        if (yPos > -0.5f && yPos < 0.3f)
        {
            sS.maxRadius = dFalloff - ((2 - yPosition) / 6);
        }*/
    }
    /// <summary>
    /// Deactivate filters used when above water.
    /// </summary>
    private void DeactivateOverFilters()
    {
        //sS.enabled = false;
        vCA.enabled = false;
        activatedOv = false;
    }
}
