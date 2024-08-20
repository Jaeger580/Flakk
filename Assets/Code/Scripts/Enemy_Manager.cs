using System.Collections;
using System.Collections.Generic;
using GeneralUtility;
using GeneralUtility.GameEventSystem;
using JO.AI;
using UnityEngine;

public class Enemy_Manager : MonoBehaviour
{
    private int currentWaveIndex = 0;
    public ContractSet chosenContract;

    [SerializeField] private GameEvent levelEndEvent, gunEnterEvent, nextWaveEvent;

    private bool enemiesDoneSpawning = false;

    private List<GameObject> livingEnemiesList = new();

    [Header("Debugging/Helper")]
    public bool isDebugging;
    public float enemySpawnRadius;
    public Vector3 enemySpawnSpherePosition;

    private void Start()
    {
        if (chosenContract is null || chosenContract.items.Count == 0)
        {
            Editor_Utility.ThrowWarning($"ERR: No chosen contract!", this);
            Destroy(gameObject);
        }

        currentWaveIndex = 0;

        var gunEnterListener = gameObject.AddComponent<GameEventListener>();
        gunEnterListener.Events.Add(gunEnterEvent);
        gunEnterListener.Response = new();
        gunEnterListener.Response.AddListener(() => StartLevel(gunEnterListener));
        gunEnterEvent.RegisterListener(gunEnterListener);
    }

    private void Update()
    {
        if (enemiesDoneSpawning && livingEnemiesList.Count <= 0)
        {
            enemiesDoneSpawning = false;
            levelEndEvent?.Trigger();
            chosenContract.items[0].completed = true;
        }
    }

    private void StartLevel(GameEventListener gunEnterListener)
    {
        Destroy(gunEnterListener);
        if (chosenContract.items[0].waves.Length <= 0) { Editor_Utility.ThrowWarning("ERR: No waves found in enemyWaves array.", this); return; }
        StartCoroutine(nameof(SpawnRoutine), chosenContract.items[0].waves[currentWaveIndex]);
    }

    private IEnumerator SpawnRoutine(Enemy_Wave_Creator wave)
    {
        foreach (var formation in wave.formations)
        {//For each formation in the wave,
            if (formation.spawnRanges.Length <= 0) { Editor_Utility.ThrowWarning("ERR: Enemy formation doesn't have an assigned spawn point.", this); yield break; }

            yield return new WaitForSeconds(wave.delayBetweenFormations);   //Delay first, otherwise could cause level-end delay

            foreach (var enemyPrefab in formation.formationUnitPrefabs)
            {//for each enemy to spawn, spawn it
                Spawn(enemyPrefab, formation.spawnRanges[Random.Range(0, formation.spawnRanges.Length)], formation.preferredWeb);
            }
        }

        currentWaveIndex++;
        if (currentWaveIndex >= chosenContract.items[0].waves.Length) { enemiesDoneSpawning = true; yield break; }

        yield return new WaitForSeconds(chosenContract.items[0].delayBetweenWaves);

        nextWaveEvent?.Trigger();
        StartCoroutine(nameof(SpawnRoutine), chosenContract.items[0].waves[currentWaveIndex]);
    }

    private void Spawn(GameObject _unit, Vector3 spawnPosition, WebSelection chosenWeb)
    {
        Vector3 newUnitPos = new Vector3(
            spawnPosition.x + Random.Range(-enemySpawnRadius, enemySpawnRadius),
            spawnPosition.y + Random.Range(-enemySpawnRadius, enemySpawnRadius),
            spawnPosition.z + Random.Range(-enemySpawnRadius, enemySpawnRadius));

        var newUnit = Instantiate(_unit, newUnitPos, _unit.transform.rotation);
        livingEnemiesList.Add(newUnit);

        if (chosenWeb == WebSelection.NONE) chosenWeb = WebSelection.WEB1;
        var ai = newUnit.GetComponent<EnemyAI>();
        ai.SetChosenWeb(chosenWeb);
        ai.EnemyDeathEvent += () => RemoveDead(newUnit);
    }

    private void RemoveDead(GameObject unit)
    {
        livingEnemiesList.Remove(unit);
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

    [ContextMenu("Manual Start")]
    private void ManualStart()
    {
        if (chosenContract.items[0].waves.Length <= 0) { Editor_Utility.ThrowWarning("ERR: No waves found in enemyWaves array.", this); return; }
        StartCoroutine(nameof(SpawnRoutine), chosenContract.items[0].waves[currentWaveIndex]);
    }
}
