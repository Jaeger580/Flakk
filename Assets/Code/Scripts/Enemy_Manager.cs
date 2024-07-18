using System.Collections;
using System.Collections.Generic;
using GeneralUtility;
using GeneralUtility.GameEventSystem;
using JO.AI;
using UnityEngine;

public class Enemy_Manager : MonoBehaviour
{
    private int currentWaveIndex = 0;
    public Enemy_Wave_Creator[] enemyWaves;
    public float waveTimeInterval;

    [SerializeField] private GameEvent levelEndEvent, gunEnterEvent;

    [Header("Debugging/Helper")]
    public bool isDebugging;
    public float enemySpawnRadius;
    public Vector3 enemySpawnSpherePosition;

    private void Start()
    {
        currentWaveIndex = 0;

        var gunEnterListener = gameObject.AddComponent<GameEventListener>();
        gunEnterListener.Events.Add(gunEnterEvent);
        gunEnterListener.Response = new();
        gunEnterListener.Response.AddListener(() => StartLevel(gunEnterListener));
        gunEnterEvent.RegisterListener(gunEnterListener);
    }

    [ContextMenu("Manual Start")]
    private void ManualStart()
    {
        if (enemyWaves.Length <= 0) { Editor_Utility.ThrowWarning("ERR: No waves found in enemyWaves array.", this); return; }
        StartCoroutine(nameof(SpawnRoutine), enemyWaves[currentWaveIndex]);
    }

    private void StartLevel(GameEventListener gunEnterListener)
    {
        Destroy(gunEnterListener);
        if (enemyWaves.Length <= 0) { Editor_Utility.ThrowWarning("ERR: No waves found in enemyWaves array.", this); return; }
        StartCoroutine(nameof(SpawnRoutine), enemyWaves[currentWaveIndex]);
    }

    private IEnumerator SpawnRoutine(Enemy_Wave_Creator wave)
    {
        foreach(var formation in wave.formations)
        {
            if (formation.spawnRanges.Length <= 0) { Editor_Utility.ThrowWarning("ERR: Enemy formation doesn't have an assigned spawn point.", this); yield break; }

            foreach (var enemyPrefab in formation.formationUnitPrefabs)
            {
                Spawn(enemyPrefab, formation.spawnRanges[Random.Range(0, formation.spawnRanges.Length)], formation.preferredWeb);
            }

            yield return new WaitForSeconds(wave.delayBetweenFormations);
        }

        yield return new WaitForSeconds(waveTimeInterval);

        currentWaveIndex++;
        if (currentWaveIndex >= enemyWaves.Length) { levelEndEvent.Trigger(); yield break; }

        StartCoroutine(nameof(SpawnRoutine), enemyWaves[currentWaveIndex]);
    }

    private void Spawn(GameObject _unit, Vector3 spawnPosition, WebSelection chosenWeb)
    {
        Vector3 newUnitPos = new Vector3(
            spawnPosition.x + Random.Range(-enemySpawnRadius, enemySpawnRadius),
            spawnPosition.y + Random.Range(-enemySpawnRadius, enemySpawnRadius),
            spawnPosition.z + Random.Range(-enemySpawnRadius, enemySpawnRadius));

        var newUnit = Instantiate(_unit, newUnitPos, _unit.transform.rotation);
        if (chosenWeb == WebSelection.NONE) chosenWeb = WebSelection.WEB1;
        newUnit.GetComponent<EnemyAI>().SetChosenWeb(chosenWeb);
    }

    private void OnDrawGizmos()
    {
        if (isDebugging)
        {
            //Show which position you'd like the enemy to spawn
            var color = Color.yellow;
            color.a = 0.25f;
            Gizmos.color = color;
            Gizmos.DrawSphere(enemySpawnSpherePosition, enemySpawnRadius);
        }
    }
}
