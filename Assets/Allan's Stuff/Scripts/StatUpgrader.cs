using UnityEngine;
using UnityEngine.UIElements;
using GeneralUtility.VariableObject;
using System.Collections.Generic;

public class StatUpgrader : MonoBehaviour
{
    [SerializeField] private FloatReference statToUpgrade;
    [SerializeField] private float amtToUpgradeBy;

    [Tooltip("No spaces, camelcase. Must directly and perfectly match the term used in the UI.")]
    [SerializeField] private string statName;  //magic strings aren't great, fix after prototype

    private Label currentLabel, nextLabel;

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var statContainer = root.Q<VisualElement>($"{statName}Container");
        statContainer.Q<Button>("Upgrade").clicked += TryUpgradeStat;

        currentLabel = statContainer.Q<Label>("CurrentAmt");
        nextLabel = statContainer.Q<Label>("NextAmt");

        UpdateUI();
    }

    private void TryUpgradeStat()
    {
        statToUpgrade.Value += amtToUpgradeBy;
        UpdateUI();
    }

    private void UpdateUI()
    {
        currentLabel.text = $"{statToUpgrade.Value}";
        nextLabel.text = $"{statToUpgrade.Value + amtToUpgradeBy}";
    }

    public void ResetStat()
    {
        statToUpgrade.Value = 0f;
        UpdateUI();
    }
}