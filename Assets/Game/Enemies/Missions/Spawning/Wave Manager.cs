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
    public static int defeatedEnemies;

    private static List<GameObject> livingEnemies = new();

    private void Start()
    {
        waveIndex = 0;
        currentWave = enemyWaves[waveIndex];
        currentEnemies = 0;
        defeatedEnemies = 0;
        livingEnemies.Clear();


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

        //if (!isSpawning && waveIndex == enemyWaves.Count - 1 && currentEnemies == defeatedEnemies) 
        //{
        //    if (!missionOver) 
        //    {
        //        EndMission();
        //        missionOver = true;
        //    }
        //}
        if (!isSpawning && waveIndex == enemyWaves.Count - 1 && livingEnemies.Count <= 0) 
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

            livingEnemies.Add(newEnemy);

            newEnemy.GetComponentInChildren<SplineAnimate>().Container = enemy.splineToFollow;

            // Used Evaluate to get the world position
            enemy.splineToFollow.Evaluate(0f, out var position, out var tangent, out var up);

            // Used to get the rotation of the starting knot of the starting spline.
            // Could have maybe used the tangent or up variable from the evaluate function above,
            // but I could not figure out how to use them properly if it is possible.

            var firstKnot = enemy.splineToFollow.Spline.ToArray()[0];

            GameObject ship = newEnemy.GetComponentInChildren<Enemy>().gameObject;
            GameObject leadingPoint = newEnemy.GetComponentInChildren<SplineAnimate>().gameObject;

            ship.transform.position = position;

            leadingPoint.transform.position = position;
            leadingPoint.transform.rotation = firstKnot.Rotation;

            ship.transform.rotation = leadingPoint.transform.rotation;

            currentEnemies++;

            Debug.Log("Enemy Spawned: Count " + currentEnemies);

            //newEnemy.GetComponentInChildren<SimpleSpline>().
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        isSpawning = false;
    }

    private void EndMission() 
    {
        winEvent.Trigger();
        //Debug.Log("VICTORY");
    }

    public static void ReduceCount(GameObject deadEnemy) 
    {
        if (livingEnemies.Contains(deadEnemy)) 
        {
            livingEnemies.Remove(deadEnemy);
        }

        defeatedEnemies++;
        Debug.Log("Enemy SLAIN: Count " + defeatedEnemies);
    }

    private void OnDestroy()
    {
        foreach(var enemy in livingEnemies) 
        {
            Destroy(enemy);
        }
    }
}