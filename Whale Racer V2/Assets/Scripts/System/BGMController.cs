using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour {

    public AudioClip aboveWaterClip;
    public AudioClip underWaterClip;
    public AudioSource audioPlayer;
    public static BGMController bgmInst;
    void Awake()
    {
        if (bgmInst == null)
        {
            bgmInst = this;
        }
    }
    public void ToggleAboveWaterSound()
    {
        if (audioPlayer.clip != aboveWaterClip)
        {
            audioPlayer.clip = aboveWaterClip;
            audioPlayer.Play();
        }
    }
    public void ToggleUnderWaterSound()
    {
        if (audioPlayer.clip != underWaterClip)
        {
            audioPlayer.clip = underWaterClip;
            audioPlayer.Play();
        }
    }
}
