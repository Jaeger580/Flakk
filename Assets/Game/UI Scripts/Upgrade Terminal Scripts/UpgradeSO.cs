using UnityEngine;
using GeneralUtility.VariableObject;
using GeneralUtility;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Upgrades/Upgrade", fileName = "Upgrade - ", order = 0)]
public class UpgradeSO : ScriptableObject
{
    [System.Serializable]
    public struct Upgrade
    {
        [Header("Display Fields")]
        public string upgradeName;
        [TextArea(3,5)]
        public string upgradeDesc;

        [Header("Value Fields")]
        public float amtToAugmentBy;
        public AugmentType augmentType;
        public int cost;
    }
    public enum AugmentType { ADD, MULTIPLY };

    public VariableObject statToIncrease;
    public float initialStatValue;
    public int currentUpgradeIndex = 0;
    public Upgrade[] upgrades;

    public bool HasNextUpgrade()
    {
        return CheckNextUpgrade() == null;
    }

    public Upgrade? CheckNextUpgrade()
    {
        if (currentUpgradeIndex + 1 >= upgrades.Length) return null;
        return upgrades[currentUpgradeIndex];
    }

    public bool BuyUpgrade(IntReference heldCurrency)
    {
        var nextPossibleUpgrade = CheckNextUpgrade();
        if (nextPossibleUpgrade == null) return false;
        var nextUpg = (Upgrade)nextPossibleUpgrade;
        if (nextUpg.cost > heldCurrency.Value) return false;

        heldCurrency.Value -= nextUpg.cost;

        switch (nextUpg.augmentType)
        {
            case AugmentType.ADD:
                {
                    if (statToIncrease is IntVariable intStat)
                        intStat.Value += Mathf.CeilToInt(nextUpg.amtToAugmentBy);
                    else if (statToIncrease is FloatVariable floatStat)
                        floatStat.Value += nextUpg.amtToAugmentBy;
                    break;
                }
            case AugmentType.MULTIPLY:
                {
                    if (statToIncrease is IntVariable intStat)
                        intStat.Value = Mathf.CeilToInt(intStat.Value * nextUpg.amtToAugmentBy);
                    else if (statToIncrease is FloatVariable floatStat)
                        floatStat.Value *= nextUpg.amtToAugmentBy;
                    break;
                }
        }

        currentUpgradeIndex++;
        return true;
    }

    //public void IncreaseStat()
    //{
    //    if (currentUpgradeIndex >= upgrades.Count) return;
    //    currentUpgradeIndex++;
    //    var nextUpg = upgrades[currentUpgradeIndex];
    //    switch (nextUpg.augmentType)
    //    {
    //        case AugmentType.ADD:
    //            {
    //                if (statToIncrease is IntVariable intStat)
    //                    intStat.Value += Mathf.CeilToInt(nextUpg.amtToAugmentBy);
    //                else if (statToIncrease is FloatVariable floatStat)
    //                    floatStat.Value += nextUpg.amtToAugmentBy;
    //                break;
    //            }
    //        case AugmentType.MULTIPLY:
    //            {
    //                if (statToIncrease is IntVariable intStat)
    //                    intStat.Value = Mathf.CeilToInt(intStat.Value * nextUpg.amtToAugmentBy);
    //                else if (statToIncrease is FloatVariable floatStat)
    //                    floatStat.Value *= nextUpg.amtToAugmentBy;
    //                break;
    //            }
    //    }
    //}

    public void ResetStat()
    {
        //if (statToIncrease is IntVariable intStat && initialStatValue is IntVariable intStatStart)
        //    intStat.Value = intStatStart.Value;
        //else if (statToIncrease is FloatVariable floatStat && initialStatValue is FloatVariable floatStatStart)
        //    floatStat.Value = floatStatStart.Value;

        if (statToIncrease is IntVariable intStat)
            intStat.Value = Mathf.CeilToInt(initialStatValue);
        else if (statToIncrease is FloatVariable floatStat)
            floatStat.Value = initialStatValue;
    }
}
