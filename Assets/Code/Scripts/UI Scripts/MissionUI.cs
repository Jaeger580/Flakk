using UnityEngine;
using UnityEngine.UIElements;

public class MissionUI : MonoBehaviour
{
    [SerializeField] private StartMission missionStarter;
    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var startMissionButton = root.Q<Button>($"StartMission");
        startMissionButton.clicked += StartMission;
    }

    private void StartMission()
    {
        missionStarter.TriggerMissionStart();
    }
}