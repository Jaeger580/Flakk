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
        public float fireRate;
        private bool isFiring = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        private void Start()
        {
            float distToPoint = 100000000f;
            var transPos = transform.position;
            int tempPatrolIndex = 0;
            currentState = AI_STATE.IDLE;   //Unsure why this is here

            if (patrolPoints.Count <= 0)
            {//If we have no patrol points, get them from the web
                patrolPoints = Spider.instance.GetWeb((int)currentWeb).waypoints;
                currentState = AI_STATE.PATROL;
            }

            foreach (var waypoint in patrolPoints)
            {//Look for the closest waypoint
                var testDist = Vector3.Distance(transPos, waypoint.position);
                if (testDist < distToPoint)
                {
                    distToPoint = testDist;
                    tempPatrolIndex = patrolPoints.IndexOf(waypoint);
                }
            }

            currentPatrolIndex = tempPatrolIndex;   //Set my current waypoint to the closest one
            speed = constSpeed;
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

        private void TargetDetection()
        {
            Collider[] targets = Physics.OverlapSphere(transform.position, 200f, ~ignoreLayer);

            foreach (Collider t in targets)
            {

            }
        }

        private void Retarget()
        {
            GameObject newTarget = GameObject.FindWithTag("Target");

            if (!targetList.Contains(newTarget.transform))
            {
                targetList.Add(newTarget.transform);
            }
        }

        private void Aggro()
        {
            if (targetList.Count <= 0)
            {
                Retarget();
                currentState = AI_STATE.PATROL;
                return;
            }

            target = targetList[currentTargetIndex];
            targetDistance = Vector3.Distance(transform.position, target.position);

            if (targetDistance <= 30f)
            {
                currentState = AI_STATE.PATROL;
            }
            else
            {
                MoveShip();

                if (targetDistance <= 200f)
                {
                    if (!isFiring)
                    {
                        Fire();
                    }
                }
            }
        }

        private void MoveShip()
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            RaycastHit hit;
            Vector3 avoidanceDirection = Vector3.zero;

            if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleForwardDetectionDistance))
            {
                if (!isbraking)
                {
                    speed -= brake;
                    isbraking = true;
                }
            }
            else
            {
                speed = constSpeed;
                isbraking = false;
            }

            if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleForwardDetectionDistance, ~ignoreLayer))
            {
                avoidanceDirection += Vector3.Reflect(directionToTarget, hit.normal);
            }
            if (Physics.Raycast(transform.position, transform.right, out hit, obstacleUpDetectionDistance, ~ignoreLayer))
            {
                avoidanceDirection += Vector3.Reflect(directionToTarget, hit.normal);
            }
            if (Physics.Raycast(transform.position, -transform.right, out hit, obstacleDownDetectionDistance, ~ignoreLayer))
            {
                avoidanceDirection += Vector3.Reflect(directionToTarget, hit.normal);
            }
            if (Physics.Raycast(transform.position, transform.up, out hit, obstacleLeftDetectionDistance, ~ignoreLayer))
            {
                avoidanceDirection += Vector3.Reflect(directionToTarget, hit.normal);
            }
            if (Physics.Raycast(transform.position, -transform.up, out hit, obstacleRightDetectionDistance, ~ignoreLayer))
            {
                avoidanceDirection += Vector3.Reflect(directionToTarget, hit.normal);
            }

            Vector3 finalDirection = directionToTarget + avoidanceDirection * avoidForce;
            finalDirection.Normalize();
            rb.MovePosition(rb.position + finalDirection * speed * Time.deltaTime);
            Vector3 lookDirection = target.position - transform.position;

            if (lookDirection.magnitude > minimumDistance)
            {
                Quaternion targetRotation = Quaternion.LookRotation(finalDirection);
                float angleDifference = Quaternion.Angle(rb.rotation, targetRotation);
                float effectiveRotationSpeed = Mathf.Clamp(rotationSpeed * (angleDifference / 180f), 0f, rotationSpeed);
                Quaternion newRotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * effectiveRotationSpeed);
                rb.MoveRotation(newRotation);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * obstacleForwardDetectionDistance);
            Gizmos.DrawRay(transform.position, transform.right * obstacleRightDetectionDistance);
            Gizmos.DrawRay(transform.position, -transform.right * obstacleLeftDetectionDistance);
            Gizmos.DrawRay(transform.position, transform.up * obstacleUpDetectionDistance);
            Gizmos.DrawRay(transform.position, -transform.up * obstacleDownDetectionDistance);
        }

        private void Fire()
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

        override public void TakeDamage(float _damage)
        {
            health -= _damage;

            if (health <= 0)
            {
                Death();
            }
        }

        private void Death()
        {
            Destroy(gameObject);
        }
    }
}
