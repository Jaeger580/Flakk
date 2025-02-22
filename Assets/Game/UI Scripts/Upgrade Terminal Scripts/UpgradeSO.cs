using UnityEngine;
using GeneralUtility.VariableObject;
using GeneralUtility;

[CreateAssetMenu(menuName = "Upgrades/Upgrade", fileName = "Upgrade - ", order = 0)]
public class UpgradeSO : ScriptableObject
{
    public enum AugmentType { ADD, MULTIPLY };

    [Header("Display Fields")]
    public string upgradeName;
    public string upgradeDesc;

    [Header("Value Fields")]
    public int cost;
    public bool bought;

    public VariableObject statToIncrease;
    //public VariableObject initialStatValue;
    public float initialStatValue;
    public float amtToAugmentBy;
    public AugmentType augmentType;

    public void IncreaseStat()
    {
        if (bought) return;
        bought = true;
        switch (augmentType)
        {
            case AugmentType.ADD:
            {
                if (statToIncrease is IntVariable intStat)
                    intStat.Value += Mathf.CeilToInt(amtToAugmentBy);
                else if (statToIncrease is FloatVariable floatStat)
                    floatStat.Value += amtToAugmentBy;
                break;
            }
            case AugmentType.MULTIPLY:
            {
                if (statToIncrease is IntVariable intStat)
                    intStat.Value = Mathf.CeilToInt(intStat.Value * amtToAugmentBy);
                else if (statToIncrease is FloatVariable floatStat)
                    floatStat.Value *= amtToAugmentBy;
                break;
            }
        }
    }

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
