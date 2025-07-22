/*
 * Jakob Jaeger
 * 6/5/2025
 * Simple script for having using a collider to track how many enemies are currently in a specific location.
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyFloorTrack : MonoBehaviour
{
    // Manueally input which floor the collider represents. Mostly for output.
    [SerializeField]
    private int floorCount;

    private List<Collider> enemyArray = new List<Collider>();

    [SerializeField]
    private TMP_Text floorText;

    [SerializeField]
    private string floorName;

    //private void OnTriggerEnter(Collider other)
    //{
    //    // Check if the enemy has the spline component so we only get one collider from the lead point.
    //    // This won't work well for future enemy types.
    //    if (other.gameObject.GetComponent<SimpleSpline>() != null)
    //    {
    //        //enemyCount++;
    //        enemyArray.Add(other);
    //        UpdateCount();
    //    }
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
    //    {
    //        enemyCount++;
    //        UpdateCount();
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (enemyArray.Contains(other))
    //    {
    //        //enemyCount--;
    //        enemyArray.Remove(other);
    //        UpdateCount();
    //    }
    //}

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
        //enemyCount = enemyArray.Count;
        int enemyCount = OverlapCheck();
        //Debug.Log("Floor " + floorCount + " has " + enemyCount + " enemies!");
        floorText.text = floorName + ": " + enemyCount;
    }

    private void FixedUpdate()
    {
        UpdateCount();
    }

    private int OverlapCheck() 
    {
        int enemyCount = 0;
        Collider[] colliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, LayerMask.GetMask("Enemy"));

        foreach (Collider collider in colliders) 
        {
            if (collider.gameObject.GetComponent<SimpleSpline>() != null)
            {
                enemyCount++;
            }
        }

        return enemyCount;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //if (Application.isPlaying) 
        //{
            Gizmos.DrawWireCube(transform.position, transform.localScale); ;
        //}
    }
}
