/* Jakob Jaeger
 * 05/26/2023
 * Script for the audio manager which will contain audio clips and allow them to be played from any other script.
 * Orignial templated based on Brackey's audio video.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private Sound[] sounds;

    [SerializeField] private AudioMixer mixer;

    public static AudioManager instance; // Used to easily access the game manager script

    [SerializeField]
    private Sound[] footSteps;
    private int footStepCount;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        foreach (Sound s in sounds) 
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.audioMixerGroup;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        foreach (Sound f in footSteps)
        {
            f.source = gameObject.AddComponent<AudioSource>();
            f.source.clip = f.clip;
            f.source.outputAudioMixerGroup = f.audioMixerGroup;
            f.source.volume = f.volume;
            f.source.pitch = f.pitch;
            f.source.loop = f.loop;
        }

    }

    private void Start()
    {
        //MUST BE IN START TO WORK, thanks Unity.
        //mixer.SetFloat(MagicStrings.AUDIO_PARAM_NAME_MASTER,
        //    Mathf.Log10(PlayerPrefs.GetFloat(MagicStrings.AUDIO_PARAM_NAME_MASTER, 0.01f)) * 20f);
        //mixer.SetFloat(MagicStrings.AUDIO_PARAM_NAME_BGM,
        //    Mathf.Log10(PlayerPrefs.GetFloat(MagicStrings.AUDIO_PARAM_NAME_BGM, 0.01f)) * 20f);
        //mixer.SetFloat(MagicStrings.AUDIO_PARAM_NAME_SFX,
        //    Mathf.Log10(PlayerPrefs.GetFloat(MagicStrings.AUDIO_PARAM_NAME_SFX, 0.01f)) * 20f);
        footStepCount = footSteps.Length;

        Play("BGM");
    }

    public void Play (string name) 
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        // Should make the audio clip only play once if called multiple times.
        if (!s.source.isPlaying) 
        {
            s.source.Play();
        }
    }

    public void ForcePlay(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        // Should make the audio clip only play once if called multiple times.
        s.source.PlayOneShot(s.clip);
    }

    public void PlayRando() 
    {
        int rand = UnityEngine.Random.Range(0, footStepCount);
        Sound f = footSteps[rand];

        f.source.Play();
    }

    public void Pause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        // Should make the audio clip only pause once if called multiple times.
        if (s.source.isPlaying)
        {
            s.source.Pause();
        }
    }

    public void setVolume(string name, float vol)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        // Should make the audio clip only pause once if called multiple times.
        s.source.volume = vol;
    }
}

static public class MagicStrings
{
    public const string AUDIO_PARAM_NAME_MASTER = "MasterVol",
        AUDIO_PARAM_NAME_BGM = "MusicVol",
        AUDIO_PARAM_NAME_SFX = "EffectVol";
} 
