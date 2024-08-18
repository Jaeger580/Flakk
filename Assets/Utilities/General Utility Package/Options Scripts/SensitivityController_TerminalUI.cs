using GeneralUtility.Options;

public class SensitivityController_TerminalUI : SensitivityController, IUIScreenRefresh
{
    public void RefreshUI()
    {
        InitUI();
    }
}
