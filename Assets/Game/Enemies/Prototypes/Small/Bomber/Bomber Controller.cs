using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberController : Enemy
{
    protected override void FixedUpdate()
    {
        if (canShoot) 
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, attackRadius, -transform.up, out hit, attackRange))
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
}