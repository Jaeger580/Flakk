using UnityEngine;
using UnityEngine.UIElements;
using GeneralUtility.VariableObject;
using GeneralUtility;

public class FloatStatUpgrader : StatUpgrader
{
    [SerializeField] private FloatReference statToUpgrade;
    [SerializeField] private float amtToUpgradeBy;
    [SerializeField] private float min, max;
    private float initialAmt;

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
        currentLabel.text = statToUpgrade.Value.ToString("0.##");
        nextLabel.text = CalcStatUpgrade().ToString("0.##");
        if (currentCurrency.Value < upgradeCost) upgradeBtn.SetEnabled(false);
    }

    override public void ResetStat()
    {
        statToUpgrade.Value = initialAmt;
        UpdateUI();
    }

    private float CalcStatUpgrade()
    {
        var newVal = statToUpgrade.Value + amtToUpgradeBy;
        if (newVal >= max) newVal = max;
        else if (newVal <= min) newVal = min;

        return newVal;
    }
}
