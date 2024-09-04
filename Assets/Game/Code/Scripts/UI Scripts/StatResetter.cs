using UnityEngine;
using UnityEngine.UIElements;

public class StatResetter : MonoBehaviour, IUIScreenRefresh
{
    private void Start()
    {
        RefreshUI();
    }

    private void ResetStats()
    {
        foreach (var upgrader in GetComponents<StatUpgrader>())
        {//Unoptimized but likely never to be an issue, fix after prototype if necessary
            upgrader.ResetStat();
        }
    }

    public void RefreshUI()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var resetButton = root.Q<Button>("Reset");
        resetButton.clicked += ResetStats;
    }
}