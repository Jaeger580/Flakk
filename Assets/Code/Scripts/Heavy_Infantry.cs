using System.Collections;
using System.Collections.Generic;
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
                return;
            }

            target = patrolPoints[currentPatrolIndex];
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance <= 2f)
            {
                UpdatePatrol();
            }
            else
            {
                MoveShip();
            }
        }

        private void UpdatePatrol()
        {
            patrolPoints = Spider.instance.GetWeb((int)currentWeb).waypoints;
            currentPatrolIndex++;

            if (currentPatrolIndex >= patrolPoints.Count)
            {
                currentPatrolIndex = 0;
                //Patrol is complete
                currentState = AI_STATE.AGGRO;
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

                GameObject projectile = null;
                projectile = Instantiate(projectilePrefab, fire_Point.position, fire_Point.rotation);

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
