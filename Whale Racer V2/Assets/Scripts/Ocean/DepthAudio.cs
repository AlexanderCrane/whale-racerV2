using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DepthAudio: MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioMixerGroup masterVolume;
    public AudioMixerGroup musicVolume;
    public AudioMixerGroup gameVolume;
    public AudioMixerGroup envirVolume;

    //Overwater Presets
    public float o_musicVolume = 0.0f;
    public float o_gameVolume = 0.0f;
    public float o_envirVolume = 0.0f;
    //Underwater Presets
    public float u_musicVolume = -3.0f;
    public float u_gameVolume = 0.0f;
    public float u_envirVolume = -1.0f;

    /*private void Awake()
    {
        masterVolume = audioMixer.FindMatchingGroups("Master")[0];
        musicVolume = audioMixer.FindMatchingGroups("Music")[0];
        musicVolume = audioMixer.FindMatchingGroups("Game")[0];
    }*/
    /// <summary>
    /// Adjust volume to whale position
    /// <param name="yPos">Y-Position of Camera</param>
    /// </summary>
    public void AdjustAudio(float yPos)
    {
        if (yPos < -0.1f)
        {
            audioMixer.SetFloat("mVolume", u_musicVolume);
            audioMixer.SetFloat("mCutoff", 1000);
            audioMixer.SetFloat("gVolume", u_gameVolume);
            audioMixer.SetFloat("eVolume", u_envirVolume);
        }
        else
        {
            audioMixer.SetFloat("mVolume", o_musicVolume);
            audioMixer.SetFloat("mCutoff", 5000);
            audioMixer.SetFloat("gVolume", o_gameVolume);
            audioMixer.SetFloat("eVolume", u_envirVolume);
        }
    }
    /// <summary>
    /// set music group volume
    /// <param name="audioSource">Music volume float parameter</param>
    /// </summary>
    public void SetMusicVolume(float music)
    {
        audioMixer.SetFloat("mVolume", music);
        o_musicVolume = music;
        u_musicVolume = music - 3.0f;
    }
    /// <summary>
    /// set game group volume
    /// <param name="game">Game volume float parameter</param>
    /// </summary>
    public void SetGameVolume(float game)
    {
        audioMixer.SetFloat("gVolume", game);
        o_gameVolume = game;
        u_gameVolume = game;
    }
    /// <summary>
    /// set environment group volume
    /// <param name="environment">Enviro volume float parameter</param>
    /// </summary>
    public void SetEnvironmentVolume(float environment)
    {
        audioMixer.SetFloat("eVolume", environment);
        o_envirVolume = environment;
        u_envirVolume = environment - 1.0f;
    }
    /// <summary>
    /// Clears volume parameters and returns to snapshot
    /// </summary>
    public void ClearVolume()
    {
        audioMixer.ClearFloat("musicVol");
    }
}
