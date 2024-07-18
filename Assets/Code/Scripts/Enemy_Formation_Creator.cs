using UnityEngine;

[CreateAssetMenu(fileName = "EF - ", menuName = "Wave System/Enemy Formation")]
public class Enemy_Formation_Creator : ScriptableObject
{
    public GameObject[] formationUnitPrefabs;
    public Vector3[] spawnRanges;
    public WebSelection preferredWeb;
}