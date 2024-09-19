/*  Jakob Jaeger
 *  09/19/2024
 *  Script that will go onto the heads of pillar enemies
 *  Handles each pillar segements movement and other functions.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarEnemy : Enemy
{
    [SerializeField]
    private float rotateSpeed = 60f;    // Rotate speed in degrees per second

    // Variables for handling random orbits
    [SerializeField]
    private float randomTimer = 5f;
    [SerializeField]
    private float ranRadius = 5f;
    private float ranTimerCount = 0f;
    private Vector3 ranDirection = Vector3.up;

    protected override void Update()
    {
        if (ranTimerCount < randomTimer)
        {
            ranTimerCount += Time.deltaTime;
        }
        else
        {
            ranTimerCount = 0f;
            ranDirection = Random.insideUnitSphere;
            Debug.Log("Direction Changed");
        }


    }

    protected override void Move() 
    {
        // By default, this method will make the enemy follow the leadPoint at a 1:1 rate.
        // Both for position and rotation.
        var targetPos = leadPoint.transform.position;

        // Matches postion to the leading point
        //this.gameObject.transform.position = targetPos;

        // Matches rotation to the leading point. Note this sibling relationship was set up so this exact feature would be optional.
        //this.gameObject.transform.rotation = leadPoint.transform.rotation;

        //gameObject.transform.RotateAround(targetPos, ranDirection/2, rotateSpeed * Time.deltaTime);


        Vector3 ranOffset = ranDirection * ranRadius;
        Vector3 offsetTarget = leadPoint.transform.position + ranOffset;

        gameObject.transform.Rotate(offsetTarget, rotateSpeed * Time.deltaTime, Space.Self);
        gameObject.transform.localPosition += transform.forward * rotateSpeed * Time.deltaTime; 

    }
}
