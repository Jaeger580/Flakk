using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave - ", menuName = "Wave System/Wave")]
public class Enemy_Wave_Creator : ScriptableObject
{
    public Enemy_Formation_Creator[] formations;
    public float delayBetweenFormations;
}
