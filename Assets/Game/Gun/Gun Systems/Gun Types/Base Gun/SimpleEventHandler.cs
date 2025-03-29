using GeneralUtility.GameEventSystem;
using UnityEngine;

abstract public class SimpleEventHandler : MonoBehaviour
{
    [SerializeField] private GameEvent[] events;

    protected void Awake()
    {
        var listener = gameObject.AddComponent<GameEventListener>();
        listener.Response = new();
        listener.Response.AddListener(() => HandleEvent());
        foreach (var evt in events)
        {
            listener.Events.Add(evt);
            evt.RegisterListener(listener);
        }
    }
    abstract protected void HandleEvent();
}
