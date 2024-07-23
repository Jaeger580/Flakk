using UnityEngine;
using UnityEngine.UIElements;

public class StartUI : MonoBehaviour, IUIScreenRefresh
{
    //[SerializeField] private StartMission missionStarter;

    private void Start()
    {
        RefreshUI();
    }

    private void ShowManual()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var manualScreen = root.Q<VisualElement>("ManualParent");
        manualScreen.AddToClassList("manualTabTransition");
    }

    private void HideManual()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var manualScreen = root.Q<VisualElement>("ManualParent");
        manualScreen.RemoveFromClassList("manualTabTransition");
    }

    public void RefreshUI()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var manualTab = root.Q<Button>($"ManualTab");
        var backTab = root.Q<Button>($"BackTab");
        manualTab.clicked += ShowManual;
        backTab.clicked += HideManual;
    }
}
