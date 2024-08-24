using GeneralUtility.GameEventSystem;
using UnityEngine;

public class ReticleZoomCamFix : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameEvent zoomEnterEvent, zoomExitEvent;

    private void Awake()
    {
        var zoomEnterListener = gameObject.AddComponent<GameEventListener>();
        zoomEnterListener.Events.Add(zoomEnterEvent);
        zoomEnterListener.Response = new();
        zoomEnterListener.Response.AddListener(() => Zoomer(true));
        zoomEnterEvent.RegisterListener(zoomEnterListener);

        var zoomExitListener = gameObject.AddComponent<GameEventListener>();
        zoomExitListener.Events.Add(zoomExitEvent);
        zoomExitListener.Response = new();
        zoomExitListener.Response.AddListener(() => Zoomer(false));
        zoomExitEvent.RegisterListener(zoomExitListener);
    }

    private void Zoomer(bool zoomed)
    {
        cam.fieldOfView = zoomed ? 36f : 52f;

        Vector3 camPos = cam.transform.localPosition;
        cam.transform.localPosition = new Vector3(camPos.x, camPos.y, camPos.z + (zoomed ? 2 : -2));
    }
}
