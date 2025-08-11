using GeneralUtility.GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VectorReticleToggle : MonoBehaviour
{
    [SerializeField]
    private GameObject ReticleCanvas;

    [SerializeField]
    private GameEvent GunEnter, GunExit;

    private Image dotImage;
    private void Start()
    {
        dotImage = GetComponent<Image>();

        var enterListener = gameObject.AddComponent<GameEventListener>();
        enterListener.Events.Add(GunEnter);
        enterListener.Response = new();
        enterListener.Response.AddListener(() => ShowReticle());
        GunEnter.RegisterListener(enterListener);

        var exitListener = gameObject.AddComponent<GameEventListener>();
        exitListener.Events.Add(GunExit);
        exitListener.Response = new();
        exitListener.Response.AddListener(() => HideReticle());
        GunExit.RegisterListener(exitListener);

        HideReticle();
    }

    private void HideReticle()
    {
        //ReticleCanvas.SetActive(false);
        dotImage.enabled = false;
    }

    private void ShowReticle()
    {
        //ReticleCanvas.SetActive(true);
        dotImage.enabled=true;
    }
}
