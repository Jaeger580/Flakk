/* Jakob Jaeger
 * 05/26/2023
 * Contains a class for sounds which will be used by another audio manager script.
 * Orignial templated based on Brackey's audio video.
*/


using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public AudioMixerGroup audioMixerGroup;

    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;

    public bool loop;

    //[HideInInspector]
    public AudioSource source;
}