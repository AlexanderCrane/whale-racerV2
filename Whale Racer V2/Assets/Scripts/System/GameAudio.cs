using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameAudio: MonoBehaviour
{
    [SerializeField] private AudioMixer master;
    [SerializeField] private AudioSource auSource;
    public AudioClip clip;
    public AudioClip startClip;
    public string mixerGroup;
    public float musicVolume = 0.0f;
    public float gameVolume = 0.0f;
    public float envirVolume = 0.0f;

    private bool routineRunning = false;

    /// <summary>
    /// Adjust music volume from main menu via slider
    /// <param name="val">slider value for music master</param>
    /// </summary>
    public void SetMusicVolume(float val)
    {
        musicVolume = val;
        master.SetFloat("mVolume", musicVolume);
    }
    /// <summary>
    /// Adjust game volume from main menu via slider
    /// <param name="val">slider value for game sfx master</param>
    /// </summary>
    public void SetGameVolume(float val)
    {
        gameVolume = val;
        master.SetFloat("gVolume", gameVolume);
    }
    /// <summary>
    /// Adjust environemnt volume from main menu via slider
    /// <param name="val">slider value for environment master</param>
    /// </summary>
    public void SetEnvirVolume(float val)
{
        envirVolume = val;
        master.SetFloat("eVolume", envirVolume);
    }
    /// <summary>
    /// Get Music Volume
    /// </summary>
    public float GetMusicVolume()
    {
        return musicVolume;
    }
    /// <summary>
    /// Get Game SFX Volume
    /// </summary>
    public float GetGameVolume()
    {
        return gameVolume;
    }
    /// <summary>
    /// Get Environment Volume
    /// </summary>
    public float GetEnvirVolume()
    {
        return envirVolume;
    }
    /// <summary>
    /// Clears the audio master to previous snapshot
    /// </summary>
    public void ClearAudio()
    {
        master.ClearFloat("musicVol");
    }
    /// <summary>
    /// Initializes new sound clip
    /// </summary>
    public void PlayAudioClip()
    {
        auSource.clip = clip;
        auSource.outputAudioMixerGroup = master.FindMatchingGroups(mixerGroup)[0];
        auSource.Play();
        if (!routineRunning)
        {
            StartCoroutine("AudioPlayShortTime");
        }
        
    }
    /// <summary>
    /// Plays a sound to gauge volume for user
    /// </summary>
    private IEnumerator AudioPlayShortTime()
    {
        //auSource.Stop();
        yield return new WaitForSeconds(3.0f);
        auSource.Stop();
        auSource.clip = startClip;
        auSource.outputAudioMixerGroup = master.FindMatchingGroups("Master")[0];
        auSource.Play();
        StopAllCoroutines();
        yield return false;
    }
}
