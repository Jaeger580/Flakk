using UnityEngine;
using UnityEngine.UIElements;

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
        AudioManager.instance.ForcePlay(clipName, AudioManager.instance.UISounds);
    }

    public void AddSFXWorkaround(Button btn)
    {//stupid bullshit, loadout options refuse to trigger their click sfx for some reason so w/e this will do
        btn.RegisterCallback<MouseOverEvent, string>(TriggerSFX, MagicStrings.BTN_HOVER);
        btn.clicked += () => TriggerSFX(MagicStrings.BTN_CLICK);
    }

    public void TriggerSFX(string clipName)
    {
        AudioManager.instance.ForcePlay(clipName, AudioManager.instance.UISounds);
    }

    public void RefreshUI()
    {
        AttachSounds();
    }
}