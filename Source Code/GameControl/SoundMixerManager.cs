using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    public static SoundMixerManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;

    private const string MASTER_VOLUME = "Master Volume";
    private const string MUSIC_VOLUME = "Music Volume";
    private const string SFX_VOLUME = "Sound Effects Volume";

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    public void SetMasterVolume(float volume)
    {
        //audioMixer.SetFloat(MASTER_VOLUME, volume);
        audioMixer.SetFloat(MASTER_VOLUME, Mathf.Log10(volume) * 20f);
    }
    
    public void SetMusicVolume(float volume)
    {
        //audioMixer.SetFloat(MUSIC_VOLUME, volume);
        audioMixer.SetFloat(MUSIC_VOLUME, Mathf.Log10(volume) * 20f);
    }

    public void SetSFXVolume(float volume)
    {
        //audioMixer.SetFloat(SFX_VOLUME, volume);
        audioMixer.SetFloat(SFX_VOLUME, Mathf.Log10(volume) * 20f);
    }

    public float GetMasterVolume()
    {
        audioMixer.GetFloat(MASTER_VOLUME, out float volume);
        return Mathf.Pow(10f, volume / 20f);
    }

    public float GetMusicVolume()
    {
        audioMixer.GetFloat(MUSIC_VOLUME, out float volume);
        return Mathf.Pow(10f, volume / 20f);
    }

    public float GetSFXVolume()
    {
        audioMixer.GetFloat(SFX_VOLUME, out float volume);
        return Mathf.Pow(10f, volume / 20f);
    }
}
