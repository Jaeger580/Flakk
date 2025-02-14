using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberController : Enemy
{
    [SerializeField]
    private int hitsToStun = 3;
    private int hitCount = 0;

    private bool burstReady = true;

    protected override void FixedUpdate()
    {
        if (canShoot) 
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, attackRadius, -transform.up, out hit, attackRange))
            {
                if (hit.transform.gameObject.layer == targetLayer)
                {
                    // If burst ready, shoot as normal
                    if (burstReady) 
                    {
                        Attack(hit, gunsList);
                        fireRateTimer = 0;
                        canShoot = false;
                    }
                    // else reset canShoot and start the next burst cycle.
                    else if (!burstReady) 
                    {
                        StartSweep();
                    }
                }
            }
        }
        
    }

    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        Debug.DrawLine(transform.position, transform.position + -transform.up * attackRange);
        Gizmos.DrawWireSphere(transform.position + -transform.up * attackRange, attackRadius);
    }

    // sets a boolean to tell the enemy to attack the next chance they get. Resets counter of the number of hits taken before sweep is cancelled.
    private void StartSweep() 
    {
        fireRateTimer = 0;
        canShoot = false;
        burstReady = true;
        hitCount = 0;
    }

    protected override void OnHit()
    {
        if (burstReady) 
        {
            hitCount++;

            if (hitCount >= hitsToStun)
            {
                burstReady = false;
            }
        }
    }

    // 
    public bool Sweeping { get { return burstReady; } }
}