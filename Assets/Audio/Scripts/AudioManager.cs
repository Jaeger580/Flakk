using GeneralUtility.GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private GameEvent gunEnter;
    [SerializeField]
    private GameEvent gunExit;

    [SerializeField]
    private AudioSource ambience;

    private float startVolume;


    private void Awake()
    {
        var enterListener = gameObject.AddComponent<GameEventListener>();
        enterListener.Events.Add(gunEnter);
        enterListener.Response = new();
        //enterListener.Response.AddListener(() => StopAmbience());
        enterListener.Response.AddListener(() => StartCoroutine(FadeOut(2.5f, 0f)));

        gunEnter.RegisterListener(enterListener);

        var exitListener = gameObject.AddComponent<GameEventListener>();
        exitListener.Events.Add(gunExit);
        exitListener.Response = new();
        //exitListener.Response.AddListener(() => PlayAmbience());
        exitListener.Response.AddListener(() => StartCoroutine(FadeIn(3f, startVolume)));
        gunExit.RegisterListener(exitListener);
    }

    private void Start()
    {
        startVolume = ambience.volume;
    }

    //private void PlayAmbience() 
    //{
    //    ambience.Play();
    //}

    //private void StopAmbience()
    //{
    //    ambience.Stop();
    //}

    private IEnumerator FadeOut(float duration, float targetVolume) 
    {
        float currentTime = 0;
        float start = ambience.volume;

        while (currentTime < duration) 
        {
            currentTime += Time.deltaTime;
            ambience.volume = Mathf.Lerp(start, targetVolume, currentTime/duration);
            yield return null;
        }

        yield break;

    }

    private IEnumerator FadeIn(float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = ambience.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            ambience.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }

        yield break;

    }
}
