using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyFighterController : Enemy
{
    [SerializeField]
    private GameObject[] leftGuns;

    [SerializeField]
    private GameObject[] rightGuns;

    protected override void FixedUpdate()
    {
        // Raycast from both sides of the enemy. If Either hits, shoot in that direction. (For now, they will share a fire rate / cooldown).
        if (canShoot)
        {
            RaycastHit hitOne;
            RaycastHit hitTwo;
            if (Physics.SphereCast(transform.position, attackRadius, transform.right, out hitOne, attackRange))
            {
                if (hitOne.transform.gameObject.layer == targetLayer)
                {
                    Attack(hitOne, rightGuns);
                    fireRateTimer = 0;
                    canShoot = false;
                }
            }
            else if (Physics.SphereCast(transform.position, attackRadius, -transform.right, out hitTwo, attackRange))
            {
                if (hitTwo.transform.gameObject.layer == targetLayer)
                {
                    Attack(hitTwo, leftGuns);
                    fireRateTimer = 0;
                    canShoot = false;
                }
            }
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        Debug.DrawLine(transform.position, transform.position + transform.right * attackRange);
        Gizmos.DrawWireSphere(transform.position + transform.right * attackRange, attackRadius);

        Debug.DrawLine(transform.position, transform.position + -transform.right * attackRange);
        Gizmos.DrawWireSphere(transform.position + -transform.right * attackRange, attackRadius);
    }
}