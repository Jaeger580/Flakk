using System.Collections;
using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.UIElements;

public class MissionUI : MonoBehaviour, IUIScreenRefresh
{
    [SerializeField] private GameEvent contractEnterEvent, contractStartEvent, missionCompleteEvent;
    private bool previouslyEntered = false, afterStart = false;

    private int chosenMission = 0;
    private MissionManager missionManager;
    private MonitorInteract monitorInteract;

    private VisualElement missionStartedScreen;

    private bool inMission = false;
    private UIDocument doc;

    bool wasRegistered;

    private void Start()
    {
        wasRegistered = false;
        missionManager = FindObjectOfType<MissionManager>();
        monitorInteract = GetComponent<MonitorInteract>();

        var missionCompleteListener = gameObject.AddComponent<GameEventListener>();
        missionCompleteListener.Events.Add(missionCompleteEvent);
        missionCompleteListener.Response = new();
        missionCompleteListener.Response.AddListener(() =>
        {
            inMission = false;
            doc.enabled = true;
            RefreshUI();
            StartCoroutine(ForceExitMonitorRoutine());
        });
        missionCompleteEvent.RegisterListener(missionCompleteListener);

        RefreshUI();
    }

    private void StartMission()
    {
        inMission = true;
        if (missionStartedScreen != null)
            missionStartedScreen.style.display = DisplayStyle.Flex;
        missionManager.LoadMission(chosenMission);
        contractStartEvent?.Trigger();
        StartCoroutine(ForceExitMonitorRoutine());
    }

    private IEnumerator ForceExitMonitorRoutine()
    {//doing it like this because otherwise it doesn't immediately reflect on the screen
        yield return null;
        monitorInteract.ForceExitMonitor();
    }

    public void RefreshUI()
    {
        doc = GetComponent<UIDocument>();
        var root = doc.rootVisualElement;

        var missionList = root.Q<VisualElement>($"ContractList");
        missionList.Clear();
        var missionDesc = root.Q<Label>($"MissionDesc");
        missionDesc.text = "";
        var startMissionButton = root.Q<Button>($"StartMission");
        startMissionButton.SetEnabled(false);

        missionStartedScreen = null;
        missionStartedScreen = root.Q<VisualElement>("MissionStartedScreen");
        if (missionStartedScreen != null)
            missionStartedScreen.style.display = inMission ? DisplayStyle.Flex : DisplayStyle.None;

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

        if (wasRegistered) 
        {
            startMissionButton.clicked -= StartMission;
        }

        startMissionButton.clicked += StartMission;

        if(!wasRegistered) wasRegistered = true;

        if (!afterStart) { afterStart = true; return; }

        if (previouslyEntered) return;

        previouslyEntered = true;
        contractEnterEvent?.Trigger();
    }
}