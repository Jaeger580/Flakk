using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class BillboardUIToGunCam : MonoBehaviour
{
    [SerializeField] private Transform gunCam;
    
    [SerializeField] private GameObject canvas;
    private Transform canvasTrans;
    private RectTransform canvasRect;
    private float originalScale, newScale, trueScale;
    [SerializeField] private float distanceCoefficient = 20f;

    private void Start()
    {
        canvasTrans = canvas.transform;
        canvasRect = canvas.GetComponent<RectTransform>();
        originalScale = canvasRect.localScale.x;

        if (gunCam == null)
        {
            gunCam = FindObjectOfType<GunCamIdentifier>().transform;
        }
    }

    private void Update()
    {
        if (!canvas.activeInHierarchy) return;
        Vector3 dir = gunCam.position - canvasTrans.position;
        //dir.y = 0;
        canvasTrans.rotation = Quaternion.LookRotation(dir);

        newScale = Vector3.Distance(gunCam.position, canvasTrans.position) / distanceCoefficient;
        trueScale = originalScale * newScale;
        canvasRect.localScale = Vector3.one * trueScale;
    }
}
