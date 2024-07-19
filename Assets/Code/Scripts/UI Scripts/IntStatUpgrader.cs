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
        var root = GetComponent<UIDocument>().rootVisualElement;

        var statContainer = root.Q<VisualElement>($"{statName}Container");
        statContainer.Q<Button>("Upgrade").clicked += TryUpgradeStat;

        currentLabel = statContainer.Q<Label>("CurrentAmt");
        nextLabel = statContainer.Q<Label>("NextAmt");

        initialAmt = statToUpgrade.Value;

        UpdateUI();
    }

    private void OnDisable()
    {
        ResetStat();
    }

    override protected void TryUpgradeStat()
    {
        if (currentCurrency.Value <= 0) { Editor_Utility.ThrowWarning($"Can't upgrade without money!", this); return; }
        statToUpgrade.Value = CalcStatUpgrade();
        UpdateUI();
    }

    override protected void UpdateUI()
    {
        currentLabel.text = $"{statToUpgrade.Value}";
        nextLabel.text = $"{CalcStatUpgrade()}";
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