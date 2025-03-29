using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockEasterEgg : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private AudioSource source;
    int clipIndex = 0;
    public void Interact(object interactor)
    {
        if (clipIndex > clips.Length - 1) return;
        if (source.isPlaying) return;

        source.clip = clips[clipIndex];
        source.Play();

        clipIndex++;
    }
}
