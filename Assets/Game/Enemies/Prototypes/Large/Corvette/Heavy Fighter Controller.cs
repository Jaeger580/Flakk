using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class HeavyFighterController : Enemy
{
    [SerializeField]
    private GameObject[] leftGuns;

    [SerializeField]
    private GameObject[] rightGuns;

    // Converting arrays to list to make them easier to resize, might be able to remove arraies entirely and remove the middle man, but not sure.
    private List<GameObject> leftGunsList;
    private List<GameObject> rightGunsList;

    protected override void Start()
    {
        currenthealth = maxHealth;
        fireRateTimer = fireRate;

        permMaxSpeed = leadPoint.GetComponent<SplineAnimate>().MaxSpeed;

        targetLayer = LayerMask.NameToLayer("Weakpoint (Player)");

        leftGunsList = new List<GameObject>();
        rightGunsList = new List<GameObject>();

        foreach (GameObject gun in leftGuns)
            leftGunsList.Add(gun);
        foreach (GameObject gun in rightGuns)
            rightGunsList.Add(gun);
    }

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
                    Attack(hitOne, rightGunsList);
                    fireRateTimer = 0;
                    canShoot = false;
                }
            }
            else if (Physics.SphereCast(transform.position, attackRadius, -transform.right, out hitTwo, attackRange))
            {
                if (hitTwo.transform.gameObject.layer == targetLayer)
                {
                    Attack(hitTwo, leftGunsList);
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

    public void RemoveGun(bool isleft, GameObject attackPoint) 
    {
        if (isleft) 
        {
            leftGunsList.Remove(attackPoint);
        }
        else if (!isleft)
        {
            rightGunsList.Remove(attackPoint);
        }
    }
}