using GeneralUtility.GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private Image reticleImage;

    private void Start()
    {
        reticleImage = WholeReticle.GetComponent<Image>();

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

        //if (isActive)
        if (reticleImage.enabled)
        {
            var targetPos = targetPositionObject.transform.position;
            targetPos.y += 5f;
            var screenPos = Camera.main.WorldToScreenPoint(targetPos);

            var recticlePos = WholeReticle.transform.position;

            // Stop duplicate UI
            if (screenPos.z < 0)
            {
                screenPos *= -1;
            }

            WholeReticle.transform.position = screenPos;
        }
    }


    // Commented these out since their functions are being moved to animations
    private void HideReticle()
    {
        //ReticleCanvas.SetActive(false);
        //isActive = false;
    }

    private void ShowReticle()
    {
        //ReticleCanvas.SetActive(true);
        //isActive = true;
    }

    
}
