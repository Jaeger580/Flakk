using GeneralUtility.EditorQoL;
using GeneralUtility.GameEventSystem;
using UnityEngine;

public class VoiceOverTrigger : MonoBehaviour
{
    [SerializeField] private VoiceOverLines line;
    [SerializeField] private GameEvent triggeringEvent;

    [SerializeField] private bool overrideCurrent, playOnce;
    [ReadOnly] [SerializeField] private bool playedAlready;

    private void Start()
    {
        var eventListener = gameObject.AddComponent<GameEventListener>();
        eventListener.Events.Add(triggeringEvent);
        eventListener.Response = new();
        eventListener.Response.AddListener(() => TriggerVO());
        triggeringEvent.RegisterListener(eventListener);
    }

    private void TriggerVO()
    {
        if (playOnce && playedAlready) return;

        playedAlready = true;

        if(overrideCurrent)
            VoiceOverPlayer.instance.OverridePlayLine(line);
        else
            VoiceOverPlayer.instance.PlayLine(line);
    }
}
