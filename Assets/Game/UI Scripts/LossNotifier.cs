﻿using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LossNotifier : MonoBehaviour
{
    [SerializeField] private GameEvent lossEvent;
    //[SerializeField] private float fadeTime = 1f;
    [SerializeField] private GameObject lossScreen;
    
    
    private PlayerInput playInput;

    private void Start()
    {
        playInput = FindObjectOfType<PlayerInput>();

        var lossListener = gameObject.AddComponent<GameEventListener>();
        lossListener.Events.Add(lossEvent);
        lossListener.Response = new();
        lossListener.Response.AddListener(() => ShowLoss());
        lossEvent.RegisterListener(lossListener);
    }

    //public void StartFade()
    //{
    //    StartCoroutine(nameof(FadeIn));
    //}

    //private IEnumerator FadeIn()
    //{
    //    float journey = 0;
    //    while(journey < fadeTime)
    //    {
    //        yield return null;

    //    }
    //}

    [ContextMenu("Lose")]
    public void ShowLoss()
    {
        this.gameObject.GetComponent<AudioSource>().Play();
        lossScreen.SetActive(true);
        playInput.SwitchCurrentActionMap("UI");
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
    }

    public void RestartScene()
    {
        print("Restarting!");
        SceneManager.LoadScene(0);
    }
}
