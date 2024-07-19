using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using GeneralUtility.VariableObject;

abstract public class StatUpgrader : MonoBehaviour
{
    [Tooltip("No spaces, camelcase. Must directly and perfectly match the term used in the UI.")]
    [SerializeField] protected string statName;  //magic strings aren't great, fix after prototype
    [SerializeField] protected IntReference currentCurrency;

    protected Label currentLabel, nextLabel;

    abstract protected void TryUpgradeStat();

    abstract protected void UpdateUI();

    abstract public void ResetStat();
}