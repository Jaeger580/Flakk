using UnityEngine;
using UnityEngine.UIElements;

public class MissionUI : MonoBehaviour, I_UIScreenRefresh
{
    [SerializeField] private StartMission missionStarter;

    private void Start()
    {
        RefreshUI();
    }

    private void StartMission()
    {
        missionStarter.TriggerMissionStart();
    }

    public void RefreshUI()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var startMissionButton = root.Q<Button>($"StartMission");
        startMissionButton.clicked += StartMission;
    }
}