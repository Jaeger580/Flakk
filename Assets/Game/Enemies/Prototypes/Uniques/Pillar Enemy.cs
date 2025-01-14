/*  Jakob Jaeger
 *  09/19/2024
 *  Script that will go onto the heads of pillar enemies
 *  Handles each pillar segements movement and other functions.
*/
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PillarEnemy : Enemy
{
    [SerializeField]
    private float rotateSpeed = 60f;    // Rotate speed in degrees per second

    // Variables for handling random orbits
    [SerializeField]
    private float randomTimer = 5f;
    [SerializeField]
    private float radius = 5f;
    private float ranTimerCount = 0f;

    [SerializeField]
    private Vector3 ranDirection = Vector3.up;

    [SerializeField]
    private float range;

    public void setLeadPoint(GameObject newLeadPoint)
    {
        leadPoint = newLeadPoint;
    }

    protected override void Update()
    {
        if (ranTimerCount < randomTimer)
        {
            ranTimerCount += Time.deltaTime;
        }
        else
        {
            ranTimerCount = 0f;

            //ranDirection = new Vector3(0, Random.Range(0, 1.0f), Random.Range(0, 1.0f));
            ranDirection.Normalize();

            Debug.Log("Direction Changed");
        }


    }

    protected override void Move() 
    {
        var targetPos = leadPoint.transform.position;


        // Ask for help increasing the speed based of distance. Copy some turret code?
        transform.LookAt(targetPos);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, rotateSpeed * Time.deltaTime);
    }

    
}
