using GeneralUtility.GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private GameEvent gunEnter;
    [SerializeField]
    private GameEvent gunExit;

    [SerializeField]
    private AudioSource ambience;


    private void Awake()
    {
        var enterListener = gameObject.AddComponent<GameEventListener>();
        enterListener.Events.Add(gunEnter);
        enterListener.Response = new();
        enterListener.Response.AddListener(() => StopAmbience());
        gunEnter.RegisterListener(enterListener);

        var exitListener = gameObject.AddComponent<GameEventListener>();
        exitListener.Events.Add(gunExit);
        exitListener.Response = new();
        exitListener.Response.AddListener(() => PlayAmbience());
        gunExit.RegisterListener(exitListener);
    }

    private void PlayAmbience() 
    {
        ambience.Play();
    }

    private void StopAmbience()
    {
        ambience.Stop();
    }
}
