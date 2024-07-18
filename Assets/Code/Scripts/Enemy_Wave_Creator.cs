using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave Name", menuName = "Create Wave")]
public class Enemy_Wave_Creator : ScriptableObject
{
    public enum EnemyGroupType { LIGHT, HEAVY };
    public EnemyGroupType[] groups;

    public int numberOfLightInfantry;
    public int numberOfHeavyInfantry;

    public Vector3[] spawnRanges;
}
