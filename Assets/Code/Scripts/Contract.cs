using UnityEngine;

[CreateAssetMenu(fileName = "Contract - ", menuName = "Contract System/Contract")]
public class Contract : ScriptableObject
{
    public string contractName;
    public string contractDesc;
    public int contractReward;
    public bool completed;
    public Enemy_Wave_Creator[] waves;
    public float delayBetweenWaves;
}
