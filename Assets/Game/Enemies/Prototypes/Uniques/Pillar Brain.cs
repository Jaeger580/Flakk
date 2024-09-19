/*  Jakob Jaeger
 *  09/19/2024
 *  Script that will go onto the brain of pillar enemies
 *  Will hold an array of all the pillars it is managing
 *  Will be the one to handle the whatever happens when a pillar segement is defeated
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarBrain : Enemy
{
    protected override void Move()
    {
        // By default, this method will make the enemy follow the leadPoint at a 1:1 rate.
        // Both for position and rotation.
        var targetPos = leadPoint.transform.position;

        // Matches postion to the leading point
        this.gameObject.transform.position = targetPos;
    }
}