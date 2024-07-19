using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMission : MonoBehaviour
{
    [SerializeField] private string levelName;
    [SerializeField] private GameEvent levelEndEvent;

    private void Start()
    {
        var levelEndListener = gameObject.AddComponent<GameEventListener>();
        levelEndListener.Events.Add(levelEndEvent);
        levelEndListener.Response = new();
        levelEndListener.Response.AddListener(() => UnloadLevel(levelEndListener));
        levelEndEvent.RegisterListener(levelEndListener);
    }

    public void TriggerMissionStart()
    {
        if (SceneManager.GetSceneByName(levelName).isLoaded == false)
        {//If the HUD scene isn't loaded, then load it as an overlay
            SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        }
    }

    private void UnloadLevel(GameEventListener levelEndListener)
    {
        Destroy(levelEndListener);

        SceneManager.UnloadSceneAsync(levelName);
    }
}
