using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour
{
    // Using object type instead of Scene type since sences can not be serialized the way I want.
    [SerializeField]
    private Mission[] missions;
    public Mission[] Missions => missions;

    private string activeMission;

    //private void Start()
    //{
    //    LoadMission(0);
    //}

    // If there is an active mission, remove it and load the correct mission
    public void LoadMission(int missionNum)
    {
        if(activeMission != null) 
        {
            SceneManager.UnloadSceneAsync(activeMission);
        }

        SceneManager.LoadScene(missions[missionNum].Name(), LoadSceneMode.Additive);
        //SceneManager.LoadScene("O_BasicLevel_1.0", LoadSceneMode.Additive);
        activeMission = missions[missionNum].Name();
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
