using System.Collections;
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
    [SerializeField]
    private AudioClip clickSFX;
    [SerializeField]
    private AudioClip hoverSFX;

    [SerializeField]
    private AudioSource sfxSource;

    private void Start()
    {
        StartCoroutine(AttachSounds());
    }

    private IEnumerator AttachSounds()
    {
        yield return null;
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.Query<Button>().ForEach(AddSFX);
    }

    private void AddSFX(Button btn)
    {//RegisterCallback example. The signature of the method being called must match the callback's return value
        btn.RegisterCallback<MouseOverEvent, AudioClip>(TriggerSFX, hoverSFX);
        btn.RegisterCallback<ClickEvent, AudioClip>(TriggerSFX, clickSFX);
    }

    public void TriggerSFX(EventBase evt, AudioClip clip)
    {
        //AudioManager.instance.ForcePlay(clipName, AudioManager.instance.UISounds);
        sfxSource.PlayOneShot(clip);
    }

    public void AddSFXWorkaround(Button btn)
    {//stupid bullshit, loadout options refuse to trigger their click sfx for some reason so w/e this will do
        btn.RegisterCallback<MouseOverEvent, AudioClip>(TriggerSFX, hoverSFX);
        btn.clicked += () => TriggerSFX(clickSFX);
    }

    public void TriggerSFX(AudioClip clip)
    {
        //AudioManager.instance.ForcePlay(clipName, AudioManager.instance.UISounds);
        sfxSource.PlayOneShot(clip);
    }

    public void RefreshUI()
    {
        StartCoroutine(AttachSounds());
    }
}