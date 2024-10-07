using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoint : MonoBehaviour
{
    [SerializeField]
    private GameObject[] targetPoints;

    public PathPoint GetNextTarget()
    {
        var nextTarget = targetPoints[0];

        return nextTarget.GetComponent<PathPoint>();
    }
}