using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Manager : MonoBehaviour
{
    public int numberOfWaves;
    private int currentWaveIndex = 1;
    public Enemy_Wave_Creator[] enemyWave;
    private int currentEnemyWaveIndex;
    public List<GameObject> enemyList = new List<GameObject>();
    public float waveTimeInterval;
    private float currentWaveTime;
    private bool isRoundRunning = false;

    [Header("Debugging/Helper")]
    public bool isDebugging;
    public float enemySpawnSphereSize;
    public Vector3 enemySpawnSpherePosition;


    void Start()
    {
        currentWaveIndex = 1;
        currentWaveTime = waveTimeInterval;
        Invoke("SpawnWave", 3f);
    }


    private void Update()
    {
        if (isRoundRunning)
        {
            currentWaveTime -= Time.deltaTime;

            if(currentWaveTime <= 0)
            {
                if(currentWaveIndex == numberOfWaves)
                {
                    print("Mission Completed");
                    return;
                }

                currentWaveTime = waveTimeInterval;
                isRoundRunning = false;
                currentWaveIndex++;
                SpawnWave();
            }
        }
    }

    private void SpawnWave()
    {
        for (int i = 1; i < numberOfWaves; i++)
        {
            if(i == currentWaveIndex)
            {
                currentEnemyWaveIndex = i;
                SpawnSelector();
                return;
            }
        }
    }

    private void SpawnSelector()
    {
        // Spawn specific unit and how many of that unit.
        if(enemyWave[currentEnemyWaveIndex - 1].numberOfLightInfantry > 0)
        {
            Spawn(enemyList[0], enemyWave[currentEnemyWaveIndex - 1].numberOfLightInfantry);
        }
        if (enemyWave[currentEnemyWaveIndex - 1].numberOfHeavyInfantry > 0)
        {
            Spawn(enemyList[1], enemyWave[currentEnemyWaveIndex - 1].numberOfHeavyInfantry);
        }
    }

    private void Spawn(GameObject _unit, int _numberOfUnits)
    {
        GameObject previousUnit = _unit;
        GameObject newUnit = null;
        Vector3 newUnitPos;

        for (int i = 0; i < _numberOfUnits; i++)
        {
            if (enemyWave[currentEnemyWaveIndex - 1].spawnRanges.Length <= 0)
            {
                newUnitPos = Vector3.zero;
            }
            else
            {
                //float point = Random.Range(-50f, enemySpawnSphereSize);
                Vector3 setPosition = enemyWave[currentEnemyWaveIndex - 1].spawnRanges[0];
                newUnitPos = new Vector3(setPosition.x + Random.Range(-50f, enemySpawnSphereSize), setPosition.y + Random.Range(-50f, enemySpawnSphereSize), setPosition.z + Random.Range(-50f, enemySpawnSphereSize));
            }

            newUnit = Instantiate(_unit, newUnitPos, previousUnit.transform.rotation);
            previousUnit = newUnit;
        }

        isRoundRunning = true;
    }

    //Debugging

    private void OnDrawGizmos()
    {
        if (isDebugging)
        {
            //Show which position you'd like the enemy to spawn
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(enemySpawnSpherePosition, enemySpawnSphereSize);
        }
    }
}
