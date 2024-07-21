using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using GeneralUtility.VariableObject;
using GeneralUtility.UI;

abstract public class StatUpgrader : MonoBehaviour, IUIScreenRefresh
{
    [Tooltip("No spaces, camelcase. Must directly and perfectly match the term used in the UI.")]
    [SerializeField] protected string statName;  //magic strings aren't great, fix after prototype
    [SerializeField] protected IntReference currentCurrency;
    [SerializeField] protected int upgradeCost = 1;

    protected Label currentLabel, nextLabel;

    abstract protected void TryUpgradeStat();

    abstract protected void UpdateUI();

    abstract public void ResetStat();

    virtual public void RefreshInfo()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var statContainer = root.Q<VisualElement>($"{statName}Container");
        statContainer.Q<Button>("Upgrade").clicked += TryUpgradeStat;

        currentLabel = statContainer.Q<Label>("CurrentAmt");
        nextLabel = statContainer.Q<Label>("NextAmt");

        UpdateUI();
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
