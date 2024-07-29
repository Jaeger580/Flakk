using GeneralUtility.Options;

public class OptionsMenu_TerminalUI : OptionsMenu, IUIScreenRefresh
{
    public void RefreshUI()
    {
        InitUI();
    }
}