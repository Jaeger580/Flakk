/*
 * Jakob Jaeger
 * 6/5/2025
 * Simple script for having using a collider to track how many enemies are currently in a specific location.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFloorTrack : MonoBehaviour
{
    // Manueally input which floor the collider represents. Mostly for output.
    [SerializeField]
    private int floorCount;

    private int enemyCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the enemy has the spline component so we only get one collider from the lead point.
        // This won't work well for future enemy types.
        if (other.gameObject.GetComponent<SimpleSpline>() != null)
        {
            enemyCount++;
            UpdateCount();
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
    //    {
    //        enemyCount++;
    //        UpdateCount();
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<SimpleSpline>() != null)
        {
            enemyCount--;
            UpdateCount();
        }
    }

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
    //    {
    //        enemyCount--;
    //        UpdateCount();
    //    }
    //}

    public void UpdateCount()
    {
        Debug.Log("Floor " + floorCount + " has " + enemyCount + " enemies!");
    }

}
