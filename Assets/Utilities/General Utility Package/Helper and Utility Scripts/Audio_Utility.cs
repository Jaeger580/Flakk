using UnityEngine;
using UnityEngine.Audio;

namespace GeneralUtility
{
    public static class Audio_Utility
    {
        static public AudioMixer mainMixer;

        static public AudioSource AddAudioSource(GameObject toAddto, bool worldSpace, bool sfx)
        {
            if (mainMixer == null) mainMixer = Resources.Load<AudioMixer>("MainAudioMixer");
            AudioSource soundSource = toAddto.AddComponent<AudioSource>();
            if (worldSpace) soundSource.spatialBlend = 0.9f;
            else soundSource.spatialBlend = 0f;

            soundSource.volume = 0.75f;

            if (sfx) soundSource.outputAudioMixerGroup = mainMixer.FindMatchingGroups("SFX")[0];
            else soundSource.outputAudioMixerGroup = mainMixer.FindMatchingGroups("Music")[0];

            soundSource.playOnAwake = false;

            return soundSource;
        }
    }
}
