using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[System.Serializable]
public class WaveManager : MonoBehaviour
{
    [SerializeField]
    private List<Wave> enemyWaves = new List<Wave>();

    [SerializeField]
    private float timeBetweenWaves = 60f;
    private float waveTimer;

    [SerializeField]
    private float timeBetweenSpawns = 2f;

    private Wave currentWave;
    private int waveIndex;

    private void Start()
    {
        waveIndex = 0;
        currentWave = enemyWaves[waveIndex];
        StartCoroutine(spawnEnemies(currentWave));
        waveTimer = 0;
    }

    private void Update()
    {
        if(waveTimer < timeBetweenWaves) 
        {
            waveTimer += Time.deltaTime;
        }
        else 
        {
            currentWave = enemyWaves[waveIndex];
            StartCoroutine(spawnEnemies(currentWave));
            waveTimer = 0;
        }
    }



    // Spawns enemies from the selected wave one at a time.
    private IEnumerator spawnEnemies(Wave wave) 
    {
        var enemies = wave.getEnemies();

        foreach(waveObject enemy in enemies) 
        {
            GameObject newEnemy = Instantiate(enemy.enemy);
            newEnemy.GetComponentInChildren<SplineAnimate>().Container = enemy.splineToFollow;

            Debug.Log("Spline Containter Set?");

            //newEnemy.GetComponentInChildren<SimpleSpline>().
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}