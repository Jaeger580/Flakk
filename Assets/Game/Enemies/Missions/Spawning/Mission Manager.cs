using System.Collections;
using System.Collections.Generic;
using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour
{
    // Using object type instead of Scene type since sences can not be serialized the way I want.
    [SerializeField]
    private Mission[] missions;
    public Mission[] Missions => missions;
    [SerializeField] private GameEvent levelEndEvent;
    private Mission activeMission;
    private CurrencyGainer currencyGainer;
    private void Start()
    {
        currencyGainer = FindObjectOfType<CurrencyGainer>();
        var levelEndListener = gameObject.AddComponent<GameEventListener>();
        levelEndListener.Events.Add(levelEndEvent);
        levelEndListener.Response = new();
        levelEndListener.Response.AddListener(() => currencyGainer.GainCurrency(activeMission.CashReward()));
        levelEndEvent.RegisterListener(levelEndListener);
    }

    // If there is an active mission, remove it and load the correct mission
    public void LoadMission(int missionNum)
    {
        if(activeMission != null) 
        {
            SceneManager.UnloadSceneAsync(activeMission.Name());
        }

        activeMission = missions[missionNum];

        SceneManager.LoadScene(activeMission.Name(), LoadSceneMode.Additive);
        //SceneManager.LoadScene("O_BasicLevel_1.0", LoadSceneMode.Additive);

    }

    public Mission GetMission(int missionNum) 
    {
        return missions[missionNum];
    }
}

[System.Serializable]
public class Mission 
{
    [SerializeField]
    private string name;

    [SerializeField]
    private string description;

    [SerializeField]
    private int cashReward;

    public string Name() { return name; }

    public string Description() { return description; }

    public int CashReward() {  return cashReward; }
}
