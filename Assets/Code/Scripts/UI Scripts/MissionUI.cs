using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.UIElements;

public class MissionUI : MonoBehaviour, IUIScreenRefresh
{ 
    [SerializeField] private ContractSet contractSet, chosenContract;
    [SerializeField] private StartMission missionStarter;
    [SerializeField] private GameEvent contractEnterEvent, contractStartEvent;
    [SerializeField] private MonitorInteract monitor;
    private bool previouslyEntered = false, afterStart = false;
    private Button contractStartButton;

    private void Start()
    {
        RefreshUI();
    }

    private void StartMission()
    {
        missionStarter.TriggerMissionStart();
        contractStartEvent?.Trigger();
        monitor.TryExitMonitor();
    }

    public void RefreshUI()
    {
        ElementInit();

        if (!afterStart) { afterStart = true; return; }

        if (previouslyEntered) return;

        previouslyEntered = true;
        contractEnterEvent?.Trigger();
    }

    private void ElementInit()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        contractStartButton = root.Q<Button>($"StartMission");
        contractStartButton.clicked += StartMission;

        var contractList = root.Q<VisualElement>("ContractList");
        contractList.Clear();

        foreach(var contract in contractSet.items)
        {
            Button newContractBtn = new();
            newContractBtn.AddToClassList("textButton");
            newContractBtn.AddToClassList("statText");

            string contractName = contract.completed ? $"<s>{contract.contractName}</s>": contract.contractName;

            newContractBtn.text = contractName;
            newContractBtn.clickable.clickedWithEventInfo += delegate { ShowContractDesc(contract); ChangeContract(contract); }; //Add the ShowDesc function to its clicked functionality


            contractList.Add(newContractBtn);
        }

        //contractList.Query<Button>().ForEach(AddSFXWrapper);
    }

    private void ShowContractDesc(Contract contract)
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var labelContractName = root.Q<Label>("ContractName");
        var labelContractDesc = root.Q<Label>("ContractDescription");

        if (contract != null)
        {
            labelContractName.text = contract.contractName;
            labelContractDesc.text = contract.contractDesc;
            //labelContractReward.text = $"Reward: {contract.contractReward} RP";
            //DisplayContractObjectives(contract);
            //DisplayEnemyTypes(contract);
            //missionSummary.visible = true;
            //missionObjectivesHolder.visible = true;
            //missionEnemyIconHolder.visible = true;
            //contractStartButton.SetEnabled(true);
        }
        else
        {
            labelContractName.text = "No contract selected!";
            labelContractDesc.text = "";
            //labelContractReward.text = "";
            //missionObjectivesHolder.Clear();
            //missionEnemyIconHolder.Clear();
            //missionSummary.visible = false;
            //missionObjectivesHolder.visible = false;
            //missionEnemyIconHolder.visible = false;
            contractStartButton.SetEnabled(false);
        }
    }

    private void ChangeContract(Contract contract)
    { 
        chosenContract.items.Clear();
        //currentObjectives.items.Clear();
        bool unlocked = true;
        if (contract != null)
        {
            //foreach (Contract prereq in contract.mission.misPrerequisites)
            //{
            //    if (!completedMissions.items.Contains(prereq))
            //        unlocked = false;
            //}
        }
        else
            unlocked = false;
        if (unlocked)
        {
            contractStartButton.SetEnabled(true);
            chosenContract.items.Add(contract);
            //if (contract.mission.objectives.items != null)
            //{
            //    for (int i = 0; i < contract.mission.objectives.items.Count; i++)
            //    {
            //        ObjectiveSO m = contract.mission.objectives.items[i];
            //        currentObjectives.items.Add(m);
            //    }
            //}
        }
        else
        {
            contractStartButton.SetEnabled(false);
            chosenContract.items.Clear();
            //currentObjectives.items.Clear();
        }

        //if (contractStartButton.enabledInHierarchy)
        //{
        //    StopCoroutine(nameof(ContinuePulse));
        //    StartCoroutine(nameof(ContinuePulse));
        //}
        //else
        //{
        //    StopCoroutine(nameof(ContinuePulse));
        //    UI_Utility.HidePulse(buttonMissionPick);
        //}
    }
}