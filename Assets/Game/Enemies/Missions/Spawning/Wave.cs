using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[System.Serializable]
public class Wave
{
    // Will hold each individual enemy added to the wave.
    public List<waveObject> enemies = new List<waveObject>();
    //public List<GameObject> splinePath = new List<GameObject>();


    public List<waveObject> getEnemies() 
    {
        return enemies;
    }
}

[System.Serializable]
public class waveObject 
{
    public GameObject enemy;
    public SplineContainer splineToFollow;
}
