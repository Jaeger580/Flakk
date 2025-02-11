/* Jakob Jaeger
 * 09/17/2024
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
using UnityEngine.Splines;
using UnityEngine.UIElements;

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

    //protected bool isAlive = false;
    // Animation curve for handling how enemies move towards leadpoints
    [SerializeField]
    protected AnimationCurve moveCurve;
    [SerializeField]
    protected float rotateSpeed = 5f;

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
                    Attack(hit, attackPoints);
                    fireRateTimer = 0;
                    canShoot = false;
                }
            }
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        Debug.DrawLine(transform.position, transform.position + transform.forward * attackRange);
        Gizmos.DrawWireSphere(transform.position + transform.forward * attackRange, attackRadius);
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
        var targetRot = leadPoint.transform.rotation;

        Transform objTransform = this.gameObject.transform;

        var moveStep = moveCurve.Evaluate(Vector3.Distance(objTransform.position, targetPos));

        // Matches postion to the leading point
        //this.gameObject.transform.position = targetPos;
        objTransform.position = Vector3.MoveTowards(objTransform.position, targetPos, moveStep);


        // Matches rotation to the leading point. Note this sibling relationship was set up so this exact feature would be optional.
        //objTransform.rotation = leadPoint.transform.rotation;
        //var rotatDir = Vector3.RotateTowards(objTransform.forward, targetRot, 10f * Time.deltaTime, 0.0f);

        //objTransform.rotation = Quaternion.LookRotation(rotatDir);

        objTransform.rotation = Quaternion.Lerp(objTransform.rotation, targetRot, rotateSpeed * Time.deltaTime);
    }


    // Damage should likely be moved to another script for
    // systems involving multiple hitzones and complex enemies.
    public bool ApplyDamage(CombatPacket packet)
    {
        //Deal damage to the enemy
        int finalDamage = CombatManager.DamageCalculator(packet);
        currenthealth -= finalDamage;

        //Debug.Log(finalDamage + " final damage taken.");
        Debug.Log("Current Health " + currenthealth);
        if (currenthealth <= 0)
        {
            Death();
        }

        return true;
    }

    public virtual void SpeedMulti(float newSpeed)
    {
        var followScript = leadPoint.GetComponent<SplineAnimate>();
        float oldSpeed = followScript.MaxSpeed;

        float prevProgress = followScript.NormalizedTime;
        
        followScript.MaxSpeed = oldSpeed * newSpeed;

        followScript.NormalizedTime = prevProgress;
    }

    protected virtual void Death() 
    {
        // Proper death needs added later
        StopAllCoroutines();

        // Not being handled directly by wave manager. Could be source of future bugs?
        WaveManager.currentEnemies--;
        Destroy(transform.parent.gameObject);
    }

    protected virtual void Attack(RaycastHit hit, GameObject[] pointsOfAttack) 
    {
        StartCoroutine(BurstAttack(hit, pointsOfAttack));
    }

    protected virtual IEnumerator BurstAttack(RaycastHit hit, GameObject[] pointsOfAttack) 
    {
        var target = hit.collider.gameObject;

        for (int i = 0;  i < burstSize; i++) 
        {
            
            //var dir = transform.position - target.transform.position;

            foreach (GameObject AP in pointsOfAttack) 
            {
                var dir = AP.transform.position - target.transform.position;
                Instantiate(attackProjectile, AP.transform.position, Quaternion.LookRotation(-dir));
            }

            yield return new WaitForSeconds(shotDelay);
        }
    }
}