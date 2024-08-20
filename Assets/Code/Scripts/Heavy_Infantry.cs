using System.Collections;
using System.Collections.Generic;
using GeneralUtility;
using UnityEngine;

namespace JO.AI
{
    [RequireComponent(typeof(Rigidbody))]
    public class Heavy_Infantry : EnemyAI
    {
        public Transform fire_Point;
        public GameObject projectilePrefab;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        private void Start()
        {
            currentState = AI_STATE.IDLE;   //Unsure why this is here

            if (patrolPoints.Count <= 0)
            {//If we have no patrol points, get them from the web
                patrolPoints = Spider.instance.GetWeb((int)currentWeb).waypoints;
                currentState = AI_STATE.PATROL;
            }

            currentPatrolIndex = patrolPoints.IndexOf(FindNearest(ref patrolPoints, 0f));   //Set my current waypoint to the closest one
            speed = constSpeed;

            var mother = FindObjectOfType<MothershipHealth>().gameObject;
            var targets = mother.GetComponentsInChildren<PROTO_DamageRedirect>();   //Change later if necessary

            foreach (var target in targets)
            {
                if (targetList.Contains(target.transform)) continue;
                targetList.Add(target.transform);
            }

            Retarget();
        }

        private void FixedUpdate()
        {
            if (currentState == AI_STATE.PATROL)
            {
                Patrol();
            }
            else if (currentState == AI_STATE.AGGRO)
            {
                Aggro();
            }
        }

        private void Patrol()
        {
            if (patrolPoints.Count <= 0)
            {
                Editor_Utility.ThrowWarning($"ERR: No points in list of patrol points.", this);
                return;
            }

            if (targetList.Contains(target))
            {//if I was previously aiming at a weakpoint, go to the nearest patrol point
                recentlyAggro = true;
                currentPatrolIndex = patrolPoints.IndexOf(FindNearest(ref patrolPoints, 60f));
                UpdatePatrol();
            }

            target = patrolPoints[currentPatrolIndex];
            //print(currentPatrolIndex);
            if (target == null) return;
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance <= 2f)
            {//Every time I hit a new patrol point,
                UpdatePatrol();
            }
            else
            {
                MoveShip();
            }
        }

        private void UpdatePatrol()
        {
            //patrolPoints = Spider.instance.GetWeb((int)currentWeb).waypoints;

            var forwardPoint = currentPatrolIndex + 1;
            var backwardPoint = currentPatrolIndex - 1;

            if (forwardPoint >= patrolPoints.Count) forwardPoint = 0;
            if (backwardPoint < 0) backwardPoint = patrolPoints.Count - 1;

            var forwardAngle = Vector3.Angle(transform.forward, (patrolPoints[forwardPoint].position - transform.position).normalized);
            var backwardAngle = Vector3.Angle(transform.forward, (patrolPoints[backwardPoint].position - transform.position).normalized);

            if (backwardAngle < forwardAngle) currentPatrolIndex--;
            else currentPatrolIndex++;

            if (currentPatrolIndex >= patrolPoints.Count) currentPatrolIndex = 0;
            else if (currentPatrolIndex < 0) currentPatrolIndex = patrolPoints.Count - 1;

            if (target == null) return;

            if (!recentlyAggro)
            {//otherwise if I'm okay to be aggro again,
                if (!Retarget()) return;

                var potentialTarget = targetList[currentTargetIndex];

                if (Vector3.Distance(transform.position, potentialTarget.position) < Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) + 30f)
                {//if there's a potential weakspot closer than the next patrol point,
                    //print($"{potentialTarget.gameObject.name} : {Vector3.Distance(transform.position, potentialTarget.position)} < {patrolPoints[currentPatrolIndex].gameObject.name} : {Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position)}");
                    currentState = AI_STATE.AGGRO;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * obstacleForwardDetectionDistance);
            Gizmos.DrawRay(transform.position, transform.right * obstacleRightDetectionDistance);
            Gizmos.DrawRay(transform.position, -transform.right * obstacleLeftDetectionDistance);
            Gizmos.DrawRay(transform.position, transform.up * obstacleUpDetectionDistance);
            Gizmos.DrawRay(transform.position, -transform.up * obstacleDownDetectionDistance);
        }

        override protected void Fire()
        {
            StartCoroutine(FireChain());
        }

        private IEnumerator FireChain()
        {

            while (currentState == AI_STATE.AGGRO)
            {
                isFiring = true;
                GameObject projectile = Instantiate(projectilePrefab, fire_Point.position, fire_Point.rotation);

                if (projectile.GetComponent<Bomb>())
                {
                    projectile.GetComponent<Bomb>().Fire(target, damageOutput);
                }

                yield return new WaitForSeconds(fireRate);
            }

            isFiring = false;
        }

        override public void TakeDamage(int _damage)
        {
            health -= _damage;

            if (health <= 0)
            {
                AudioManager.instance.Play("Distant Exp");
                deathFX.Play();
                Death();
            }
        }
    }
}
