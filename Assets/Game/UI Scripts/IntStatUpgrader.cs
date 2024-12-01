using UnityEngine;
using UnityEngine.UIElements;
using GeneralUtility.VariableObject;
using GeneralUtility;

public class IntStatUpgrader : StatUpgrader
{
    [SerializeField] private IntReference statToUpgrade;
    [SerializeField] private int amtToUpgradeBy;
    [SerializeField] private int min, max;
    private int initialAmt;

    private void Start()
    {
        initialAmt = statToUpgrade.Value;

        RefreshInfo();
    }

    private void OnDisable()
    {
        ResetStat();
    }

    override protected void TryUpgradeStat()
    {
        if (!TryUpgrade()) return;
        statToUpgrade.Value = CalcStatUpgrade();
        UpdateUI();
    }

    override protected void UpdateUI()
    {
        currentLabel.text = $"{statToUpgrade.Value}";
        nextLabel.text = $"{CalcStatUpgrade()}";
        if (currentCurrency.Value < upgradeCost) upgradeBtn.SetEnabled(false);
    }

    override public void ResetStat()
    {
        statToUpgrade.Value = initialAmt;
        UpdateUI();
    }

    private int CalcStatUpgrade()
    {
        var newVal = statToUpgrade.Value + amtToUpgradeBy;
        if (newVal >= max) newVal = max;
        else if (newVal <= min) newVal = min;

        return newVal;
    }
}