using GeneralUtility.GameEventSystem;
using UnityEngine;

public class WinNotifier : MonoBehaviour
{
    [SerializeField] private GameEvent winEvent, hideEvent; //temp secondary event for resetting the win screen
    [SerializeField] private GameObject winBG;

    private void Start()
    {
        var winListener = gameObject.AddComponent<GameEventListener>();
        winListener.Events.Add(winEvent);
        winListener.Response = new();
        winListener.Response.AddListener(() => ShowWin());
        winEvent.RegisterListener(winListener);

        var hideListener = gameObject.AddComponent<GameEventListener>();
        hideListener.Events.Add(hideEvent);
        hideListener.Response = new();
        hideListener.Response.AddListener(() => ShowWin());
        hideEvent.RegisterListener(hideListener);

        HideWin();
    }

    private void HideWin()
    {
        winBG.SetActive(false);
    }

    private void ShowWin()
    {
        winBG.SetActive(true);
    }
}
