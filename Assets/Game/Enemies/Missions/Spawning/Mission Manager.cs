using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour
{
    // Using object type instead of Scene type since sences can not be serialized the way I want.
    [SerializeField]
    private Object[] missions;

    private Object activeMission;

    private void Start()
    {
        LoadMission(0);
    }

    // If there is an active mission, remove it and load the correct mission
    public void LoadMission(int missionNum)
    {
        if(activeMission != null) 
        {
            SceneManager.UnloadSceneAsync(activeMission.name);
        }

        //SceneManager.LoadScene(missions[missionNum].name, LoadSceneMode.Additive);
        SceneManager.LoadScene("O_BasicLevel_1.0", LoadSceneMode.Additive);
        activeMission = missions[missionNum];
    }
}
