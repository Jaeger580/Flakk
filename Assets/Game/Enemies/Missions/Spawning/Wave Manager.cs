using GeneralUtility.GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

    [SerializeField]
    private GameEvent winEvent;

    private Wave currentWave;
    private int waveIndex;

    private bool isSpawning = false;
    private bool missionOver = false;

    public static int currentEnemies;

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
            if(waveIndex < enemyWaves.Count - 1) 
            {
                waveIndex++;
                currentWave = enemyWaves[waveIndex];
                StartCoroutine(spawnEnemies(currentWave));
                waveTimer = 0;
            }
        }

        if (!isSpawning && waveIndex == enemyWaves.Count - 1 && currentEnemies == 0) 
        {
            if (!missionOver) 
            {
                EndMission();
                missionOver = true;
            }
        }
    }


    // Spawns enemies from the selected wave one at a time.
    private IEnumerator spawnEnemies(Wave wave) 
    {
        var enemies = wave.getEnemies();
        isSpawning = true;

        foreach(waveObject enemy in enemies) 
        {
            GameObject newEnemy = Instantiate(enemy.enemy);

            newEnemy.GetComponentInChildren<SplineAnimate>().Container = enemy.splineToFollow;

            enemy.splineToFollow.Evaluate(0f, out var position, out var tangent, out var up);

            GameObject ship = newEnemy.GetComponentInChildren<Enemy>().gameObject;
            GameObject leadingPoint = newEnemy.GetComponentInChildren<SplineAnimate>().gameObject;

            ship.transform.position = position;
            leadingPoint.transform.position = position;

            currentEnemies++;

            //Debug.Log("Spline Containter Set?");

            //newEnemy.GetComponentInChildren<SimpleSpline>().
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        isSpawning = false;
    }

    private void EndMission() 
    {
        winEvent.Trigger();
        Debug.Log("VICTORY");
    }
}