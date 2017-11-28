using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls background music.
/// </summary>
public class BGMController : MonoBehaviour {

    public AudioClip aboveWaterClip;
    public AudioClip underWaterClip;
    public AudioSource audioPlayer;
    public static BGMController bgmInst;
    /// <summary>
    /// Awake method. Creates a static instance for reference.
    /// </summary>
    void Awake()
    {
        if (bgmInst == null)
        {
            bgmInst = this;
        }
    }
    /// <summary>
    /// Starts playing abovewater sounds. To be called when camera passes above water.
    /// </summary>
    public void ToggleAboveWaterSound()
    {
        if (audioPlayer.clip != aboveWaterClip)
        {
            audioPlayer.clip = aboveWaterClip;
            audioPlayer.Play();
        }
    }
    /// <summary>
    /// Starts playing underwater sounds. To be called when camera passes under water.
    /// </summary>
    public void ToggleUnderWaterSound()
    {
        if (audioPlayer.clip != underWaterClip)
        {
            audioPlayer.clip = underWaterClip;
            audioPlayer.Play();
        }
    }
}
