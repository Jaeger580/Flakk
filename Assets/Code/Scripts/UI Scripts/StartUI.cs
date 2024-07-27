using System.Collections;
using GeneralUtility;
using GeneralUtility.EditorQoL;
using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.UIElements;

public class StartUI : MonoBehaviour, IUIScreenRefresh
{
    [SerializeField] private GameEvent startEvent, manualEvent, exitPromptEvent, terminalDescEvent, contractDescEvent;
    [SerializeField] private GameEvent exitMonitorEvent, terminalEnteredEvent, contractStartedEvent;
    private enum VoiceOverBitFlag
    {
        NONE = 0,
        MANUAL_CHECKED = 1,
        TERMINAL_EXITED = 2,
        TERMINAL_ENTERED = 4,
        CONTRACT_STARTED = 8
    }
    [EnumFlag] [SerializeField] [ReadOnly] private VoiceOverBitFlag voFlags;

    private IEnumerator Start()
    {
        var monitorExitListener = gameObject.AddComponent<GameEventListener>();
        monitorExitListener.Events.Add(exitMonitorEvent);
        monitorExitListener.Response = new();
        monitorExitListener.Response.AddListener(() => voFlags |= VoiceOverBitFlag.TERMINAL_EXITED);
        monitorExitListener.Response.AddListener(() => StartCoroutine(nameof(TerminalDescription)));
        exitMonitorEvent.RegisterListener(monitorExitListener);

        var terminalEnteredListener = gameObject.AddComponent<GameEventListener>();
        terminalEnteredListener.Events.Add(terminalEnteredEvent);
        terminalEnteredListener.Response = new();
        terminalEnteredListener.Response.AddListener(() => voFlags |= VoiceOverBitFlag.TERMINAL_ENTERED);
        terminalEnteredEvent.RegisterListener(terminalEnteredListener);

        var contractEnteredListener = gameObject.AddComponent<GameEventListener>();
        contractEnteredListener.Events.Add(contractStartedEvent);
        contractEnteredListener.Response = new();
        contractEnteredListener.Response.AddListener(() => voFlags |= VoiceOverBitFlag.CONTRACT_STARTED);
        contractStartedEvent.RegisterListener(contractEnteredListener);

        RefreshUI();

        yield return new WaitForSeconds(0.01f);

        startEvent?.Trigger();
        StartCoroutine(nameof(ManualTutorial));
    }

    private IEnumerator ManualTutorial()
    {//Wait for a few seconds to let the previous voice clip end, then if they haven't already checked the manual, tell them to
        yield return new WaitForSeconds(11f);
        if (!voFlags.HasFlag(VoiceOverBitFlag.MANUAL_CHECKED) &&
            !voFlags.HasFlag(VoiceOverBitFlag.CONTRACT_STARTED)) manualEvent?.Trigger();
        StartCoroutine(nameof(ExitPrompt));
    }

    private IEnumerator ExitPrompt()
    {//Wait for previous VO, then if they haven't exited monitor, tell them to
        yield return new WaitForSeconds(5f);
        if (!voFlags.HasFlag(VoiceOverBitFlag.TERMINAL_EXITED) &&
            !voFlags.HasFlag(VoiceOverBitFlag.CONTRACT_STARTED)) exitPromptEvent?.Trigger();
    }

    private IEnumerator TerminalDescription()
    {//Wait for previous VO, then if they haven't entered a terminal, tell them to
        yield return new WaitForSeconds(7f);

        if (!voFlags.HasFlag(VoiceOverBitFlag.TERMINAL_ENTERED) &&
            !voFlags.HasFlag(VoiceOverBitFlag.CONTRACT_STARTED)) terminalDescEvent?.Trigger();

        yield return new WaitForSeconds(12f);
        if (!voFlags.HasFlag(VoiceOverBitFlag.CONTRACT_STARTED)) contractDescEvent?.Trigger();
    }

    private void ShowManual()
    {
        voFlags |= VoiceOverBitFlag.MANUAL_CHECKED;
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

        var quitBtn = root.Q<Button>("QuitGame");
        quitBtn.clicked += Application.Quit;
    }
}
