using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.UIElements;

public class MissionUI : MonoBehaviour, IUIScreenRefresh
{
    [SerializeField] private StartMission missionStarter;
    [SerializeField] private GameEvent contractEnterEvent, contractStartEvent;
    private bool previouslyEntered = false, afterStart = false;

    private void Start()
    {
        RefreshUI();
    }

    private void StartMission()
    {
        missionStarter.TriggerMissionStart();
        contractStartEvent?.Trigger();
    }

    public void RefreshUI()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var startMissionButton = root.Q<Button>($"StartMission");
        startMissionButton.clicked += StartMission;

        if (!afterStart) { afterStart = true; return; }

        if (previouslyEntered) return;

        previouslyEntered = true;
        contractEnterEvent?.Trigger();
    }
}