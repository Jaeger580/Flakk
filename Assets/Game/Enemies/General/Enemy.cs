/* Jakob Jaeger
 * 09/17/2024
 * This is a prototype script that SHOULD NOT BE USED AFTER TESTING. A new script should be rewritten as the framework for future enemies. 
 * 
 * This script contains an abstract class that will be used as the base for all future enemies.
 * Contains values for health, move speed, ect...
 * Will contain methods handling, attacking, and other AI behaviors.
 */

using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField]
    protected GameObject leadPoint;
    [SerializeField]
    protected int maxHealth;

    protected int currenthealth;
    //[SerializeField]
    //protected float moveSpeed;    // Need to decide how moveSpeed works with how the leadPoint follows splines

    protected virtual void Start()
    {
        currenthealth = maxHealth;
    }

    protected virtual void Update()
    {
    }

    protected virtual void FixedUpdate()
    {
    }

    protected virtual void LateUpdate()
    {
        // Moving in late update seems smooth when using the follow cam in scene view,
        // but this might cause issues with complex enemy types.
        Move();
    }

    protected virtual void Move() 
    {
        // By default, this method will make the enemy follow the leadPoint at a 1:1 rate.
        // Both for position and rotation.
        var targetPos = leadPoint.transform.position;

        // Matches postion to the leading point
        this.gameObject.transform.position = targetPos;

        // Matches rotation to the leading point. Note this sibling relationship was set up so this exact feature would be optional.
        this.gameObject.transform.rotation = leadPoint.transform.rotation;
    }


    // Damage should likely be moved to another script for
    // systems involving multiple hitzones and complex enemies.
    public virtual void TakeDamage(int damage)
    {
        //Deal damage to the enemy
        currenthealth -= damage;

        if (currenthealth <= 0) 
        {
            death();
        }
    }

    protected virtual void death() 
    {

    }
}