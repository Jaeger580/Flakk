using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.UIElements;

public class MissionUI : MonoBehaviour, IUIScreenRefresh
{
    [SerializeField] private GameEvent contractEnterEvent, contractStartEvent;
    private bool previouslyEntered = false, afterStart = false;

    private int chosenMission = 0;
    private MissionManager missionManager;

    private void Start()
    {
        missionManager = FindObjectOfType<MissionManager>();

        RefreshUI();
    }

    private void StartMission()
    {
        missionManager.LoadMission(chosenMission);
        contractStartEvent?.Trigger();
    }

    public void RefreshUI()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var missionList = root.Q<VisualElement>($"ContractList");
        missionList.Clear();
        var missionDesc = root.Q<Label>($"MissionDesc");
        missionDesc.text = "";
        var startMissionButton = root.Q<Button>($"StartMission");
        startMissionButton.SetEnabled(false);

        void SwapChosenMission(int index, Mission mis, Label descLabel)
        {
            descLabel.text = mis.Description();
            chosenMission = index;
            startMissionButton.SetEnabled(true);
        }

        for (int i = 0; i < missionManager.Missions.Length; i++)
        {
            Mission mis = missionManager.Missions[i];
            var misButton = new Button();
            misButton.name = mis.Name();
            misButton.text = mis.Name();
            misButton.AddToClassList("textButton");
            misButton.AddToClassList("statText");
            int index = i;
            misButton.clicked += () => SwapChosenMission(index, mis, missionDesc);
            missionList.Add(misButton);
        }


        startMissionButton.clicked += StartMission;

        if (!afterStart) { afterStart = true; return; }

        if (previouslyEntered) return;

        previouslyEntered = true;
        contractEnterEvent?.Trigger();
    }
}