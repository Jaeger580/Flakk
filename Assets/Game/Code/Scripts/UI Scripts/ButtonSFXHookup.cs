using UnityEngine;
using UnityEngine.UIElements;

static public class MagicStrings
{
    public const string AUDIO_PARAM_NAME_MASTER = "MasterVol",
        AUDIO_PARAM_NAME_BGM = "MusicVol",
        AUDIO_PARAM_NAME_SFX = "EffectVol";

    public const string BTN_HOVER = "UI Button Hover",
        BTN_CLICK = "UI Button Click",
        BTN_ERROR = "UI Button Error";

    public const string VO_INTRO = "Intro",
        VO_MANUAL = "Manual",
        VO_EXIT_CONSOLE = "Exit Console",
        VO_DECK_DESC = "Deck Description",
        VO_TERMINAL_DESC = "Terminal Explanation",
        VO_CONTRACT_DESC = "Contract Explain",
        VO_CONTRACT_START = "Contract Start",
        VO_LOSS = "Loss",
        VO_WIN = "Win",
        VO_NEXT_WAVE = "Next Wave",
        VO_WEAKPOINT_DEATH = "Weakpoint Death";
}

public class ButtonSFXHookup : MonoBehaviour, IUIScreenRefresh
{
    private void Start()
    {
        AttachSounds();
    }

    private void AttachSounds()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.Query<Button>().ForEach(AddSFX);
    }

    private void AddSFX(Button btn)
    {//RegisterCallback example. The signature of the method being called must match the callback's return value
        btn.RegisterCallback<MouseOverEvent, string>(TriggerSFX, MagicStrings.BTN_HOVER);
        btn.RegisterCallback<ClickEvent, string>(TriggerSFX, MagicStrings.BTN_CLICK);
    }

    public void TriggerSFX(EventBase evt, string clipName)
    {
        //AudioManager.instance.ForcePlay(clipName, AudioManager.instance.UISounds);
    }

    public void AddSFXWorkaround(Button btn)
    {//stupid bullshit, loadout options refuse to trigger their click sfx for some reason so w/e this will do
        btn.RegisterCallback<MouseOverEvent, string>(TriggerSFX, MagicStrings.BTN_HOVER);
        btn.clicked += () => TriggerSFX(MagicStrings.BTN_CLICK);
    }

    public void TriggerSFX(string clipName)
    {
        //AudioManager.instance.ForcePlay(clipName, AudioManager.instance.UISounds);
    }

    public void RefreshUI()
    {
        AttachSounds();
    }
}