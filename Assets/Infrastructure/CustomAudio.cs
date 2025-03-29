using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;
using static UnityEditor.PlayerSettings;


// Contains public methods for handling reocurring tasks related to audio. Example: randomizing the pitch of any audio played.
public static class CustomAudio
{

    public static void PlayWithPitch(AudioSource source, float startPitch) 
    {
        startPitch = 1f;

        // Try randomizing the pitch before playing the clip
        float minPitch = startPitch - (startPitch * 0.05f);
        float maxPitch = startPitch + (startPitch * 0.05f);

        float ranPitch = Random.Range(minPitch, maxPitch);

        source.pitch = ranPitch;
        source.Play();
    }

    public static void PlayOnceWithPitch(AudioSource source, float startPitch)
    {
        startPitch = 1f;

        // Try randomizing the pitch before playing the clip
        float minPitch = startPitch - (startPitch * 0.05f);
        float maxPitch = startPitch + (startPitch * 0.05f);

        float ranPitch = Random.Range(minPitch, maxPitch);

        source.pitch = ranPitch;
        source.PlayOneShot(source.clip);
    }

    public static void PlayWithMinorPitch(AudioSource source, float startPitch)
    {
        startPitch = 1f;

        // Try randomizing the pitch before playing the clip
        float minPitch = startPitch - (startPitch * 0.025f);
        float maxPitch = startPitch + (startPitch * 0.025f);

        float ranPitch = Random.Range(minPitch, maxPitch);

        source.pitch = ranPitch;
        source.PlayOneShot(source.clip);
    }

    //public static void PlayClipAt(AudioClip clip, Vector3 pos) 
    //{
    //    // create temp game object with audio component and set the postion
    //    var temoGO = new GameObject("TempAudio");
    //    temoGO.transform.position = pos;
    //    var aSource = temoGO.AddComponent<AudioSource>();

    //    // Set audio source properties



    //    // Play audio then destroy the temp object
    //    aSource.Play();
    //    GameObject.Destroy(temoGO, clip.length);
    //}

    public static void PlayClipAt(AudioSource audioSource, Vector3 pos)
    {
        // Create temp game object and add an audio compontent
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = pos; // set its position
        AudioSource tempASource = tempGO.AddComponent<AudioSource>();

        // Copy desires from an already created audio source so we can set our desired setting in the inspector;
        tempASource.clip = audioSource.clip;
        tempASource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
        tempASource.mute = audioSource.mute;
        tempASource.bypassEffects = audioSource.bypassEffects;
        tempASource.bypassListenerEffects = audioSource.bypassListenerEffects;
        tempASource.bypassReverbZones = audioSource.bypassReverbZones;
        tempASource.playOnAwake = audioSource.playOnAwake;
        tempASource.loop = audioSource.loop;
        tempASource.priority = audioSource.priority;
        tempASource.volume = audioSource.volume;
        tempASource.pitch = audioSource.pitch;
        tempASource.panStereo = audioSource.panStereo;
        tempASource.spatialBlend = audioSource.spatialBlend;
        tempASource.reverbZoneMix = audioSource.reverbZoneMix;
        tempASource.dopplerLevel = audioSource.dopplerLevel;
        tempASource.rolloffMode = audioSource.rolloffMode;
        tempASource.minDistance = audioSource.minDistance;
        tempASource.spread = audioSource.spread;
        tempASource.maxDistance = audioSource.maxDistance;

        // set other aSource properties here, if desired

        // Randomise Pitch
        float curPitch = tempASource.pitch;
        float minPitch = curPitch - (curPitch * 0.05f);
        float maxPitch = curPitch + (curPitch * 0.05f);

        float ranPitch = Random.Range(minPitch, maxPitch);

        tempASource.pitch = ranPitch;

        // Play Audio then destroy the temp object.
        tempASource.Play(); // start the sound
        MonoBehaviour.Destroy(tempGO, tempASource.clip.length); // destroy object after clip duration (this will not account for whether it is set to loop)
    }
}
