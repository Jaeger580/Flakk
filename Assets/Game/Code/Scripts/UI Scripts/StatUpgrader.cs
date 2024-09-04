using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using GeneralUtility.VariableObject;
using GeneralUtility.UI;
using GeneralUtility.GameEventSystem;
using GeneralUtility;

//public class AudioManager { }

abstract public class StatUpgrader : MonoBehaviour, IUIScreenRefresh
{
    [Tooltip("No spaces, camelcase. Must directly and perfectly match the term used in the UI.")]
    [SerializeField] protected string statName;  //magic strings aren't great, fix after prototype
    [SerializeField] protected IntReference currentCurrency;
    [SerializeField] protected int upgradeCost = 1;
    [SerializeField] protected GameEvent currencyChangedEvent;

    protected Label currentLabel, nextLabel;
    protected Button upgradeBtn;

    abstract protected void TryUpgradeStat();

    abstract protected void UpdateUI();

    abstract public void ResetStat();

    virtual public void RefreshInfo()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var statContainer = root.Q<VisualElement>($"{statName}Container");
        upgradeBtn = statContainer.Q<Button>("Upgrade");
        upgradeBtn.text = $"UPGRADE : ${upgradeCost}";
        upgradeBtn.clicked += TryUpgradeStat;

        if (currentCurrency.Value < upgradeCost) upgradeBtn.SetEnabled(false);

        currentLabel = statContainer.Q<Label>("CurrentAmt");
        nextLabel = statContainer.Q<Label>("NextAmt");

        UpdateUI();
    }

    protected bool TryUpgrade()
    {
        if (currentCurrency.Value < upgradeCost)
        {
            Editor_Utility.ThrowWarning($"ERR: Currency amt ({currentCurrency.Value}) less than upgrade cost ({upgradeCost})!", this);
            //AudioManager.instance.ForcePlay(MagicStrings.BTN_ERROR, AudioManager.instance.UISounds);
            return false;
        }
        currentCurrency.Value -= upgradeCost;
        currencyChangedEvent?.Trigger();
        return true;
    }

    public void RefreshUI()
    {
        RefreshInfo();
    }
}

public interface IUIScreenRefresh
{
    public void RefreshUI();
}
