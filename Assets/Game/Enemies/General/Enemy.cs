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
using GeneralUtility.CombatSystem;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField]
    protected GameObject leadPoint;
    [SerializeField]
    protected int maxHealth;

    [SerializeField]
    protected float attackRange;
    [SerializeField]
    protected float attackRadius;
    [SerializeField]
    protected float fireRate;
    protected float fireRateTimer;
    protected bool canShoot;

    [SerializeField]
    protected float burstSize;
    [SerializeField]
    protected float shotDelay;

    [SerializeField]
    protected GameObject[] attackPoints;
    [SerializeField]
    protected GameObject attackProjectile;

    protected int currenthealth;
    protected int targetLayer;

    public int MaxHealth => throw new System.NotImplementedException();

    //[SerializeField]
    //protected float moveSpeed;    // Need to decide how moveSpeed works with how the leadPoint follows splines

    protected virtual void Start()
    {
        currenthealth = maxHealth;
        fireRateTimer = fireRate;

        targetLayer = LayerMask.NameToLayer("Weakpoint (Player)");
    }

    protected virtual void Update()
    {
        if(fireRateTimer < fireRate) 
        {
            fireRateTimer += Time.deltaTime;
            canShoot = false;
        }
        else 
        {
            canShoot = true;
        }
    }

    protected virtual void FixedUpdate()
    {
        if(canShoot) 
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, attackRadius, transform.forward, out hit, attackRange))
            {
                if (hit.transform.gameObject.layer == targetLayer)
                {
                    Attack(hit);
                    fireRateTimer = 0;
                    canShoot = false;
                }
            }
        }
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
    public bool ApplyDamage(CombatPacket packet)
    {
        //Deal damage to the enemy
        int finalDamage = CombatManager.DamageCalculator(packet);
        currenthealth -= finalDamage;

        Debug.Log(finalDamage + " final damage taken.");

        if (currenthealth <= 0)
        {
            Death();
        }

        return true;
    }

    public virtual void SpeedMulti(float newSpeed)
    {
        //Deal damage to the enemy
        var followScript = leadPoint.GetComponent<SPLineFollow>();
        float oldSpeed = followScript.MoveSpeed;

        followScript.MoveSpeed = oldSpeed * newSpeed;
    }

    protected virtual void Death() 
    {

    }

    protected virtual void Attack(RaycastHit hit) 
    {
        StartCoroutine(BurstAttack(hit));
    }

    protected virtual IEnumerator BurstAttack(RaycastHit hit) 
    {
        for (int i = 0;  i < burstSize; i++) 
        {
            var dir = transform.position - hit.transform.position;

            foreach (GameObject AP in attackPoints) 
            {
                Instantiate(attackProjectile, AP.transform.position, Quaternion.LookRotation(-dir));
            }

            yield return new WaitForSeconds(shotDelay);
        }
    }
}