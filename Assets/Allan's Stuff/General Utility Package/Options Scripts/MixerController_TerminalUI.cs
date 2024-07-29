using GeneralUtility.Options;

public class MixerController_TerminalUI : MixerController, IUIScreenRefresh
{
    public void RefreshUI()
    {
        InitUI();
    }
}
