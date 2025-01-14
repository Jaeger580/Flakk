using GeneralUtility.GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrueReticle : MonoBehaviour
{
    [SerializeField]
    private GameObject ReticleCanvas;

    [SerializeField]
    private GameObject WholeReticle;

    [SerializeField]
    private GameObject targetPositionObject;

    [SerializeField]
    private GameEvent GunEnter, GunExit;

    private bool isActive = false;

    private void Start()
    {
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

    private void Update()
    {
        if (isActive) 
        {
            var screenPos = Camera.main.WorldToScreenPoint(targetPositionObject.transform.position);

            var recticlePos = WholeReticle.transform.position;

            WholeReticle.transform.position = screenPos;
        }
    }

    private void HideReticle()
    {
        ReticleCanvas.SetActive(false);
        isActive = false;
    }

    private void ShowReticle()
    {
        ReticleCanvas.SetActive(true);
        isActive = true;
    }

    
}
