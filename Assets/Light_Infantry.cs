using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JO.AI 
{
    [RequireComponent(typeof(Rigidbody))]
    public class Light_Infantry : AI_FSM
    {
        private Rigidbody rb;

        [Header("Settings")]
        public Transform[] patrolPoints;
        public int currentPatrolIndex;
        public bool isPatrolling = false;
        public float patrolSpeed;
        public float rotationSpeed;
        public float minimumDistance = 2f;

        public Transform target;
        public LayerMask ignoreLayer;
        public float obstacleForwardDetectionDistance;
        public float obstacleUpDetectionDistance;
        public float obstacleDownDetectionDistance;
        public float obstacleLeftDetectionDistance;
        public float obstacleRightDetectionDistance;
        public float avoidForce;
        public Transform[] targetList;
        public int currentTargetIndex;
        public float targetDistance;
        private float patrolTime;
        public AI_STATE currentState;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        void Start()
        {
            currentPatrolIndex = 0;
            currentState = AI_STATE.AGGRO;
        }

        private void Update()
        {

        }

        private void FixedUpdate()
        {
            if (currentState == AI_STATE.PATROL)
            {
                Patrol();
            }
            else if(currentState == AI_STATE.AGGRO)
            {
                Aggro();
            }
        }

        private void Patrol()
        {
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
            currentPatrolIndex++;

            if (currentPatrolIndex >= patrolPoints.Length)
            {
                currentPatrolIndex = 0;
                //Patrol is complete
                currentState = AI_STATE.AGGRO;
            }
        }

        private void Aggro()
        {
            if (targetList.Length <= 0)
            {
                currentState = AI_STATE.PATROL;
                return;
            }

            target = targetList[currentTargetIndex];
            targetDistance = Vector3.Distance(transform.position, target.position);

            if (targetDistance <= 15f)
            {
                currentState = AI_STATE.PATROL;
            }
            else
            {
                MoveShip();

                if(targetDistance <= 25f)
                {
                    Fire();
                }
            }
        }

        private void MoveShip()
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            RaycastHit hit;
            Vector3 avoidanceDirection = Vector3.zero;

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
            rb.MovePosition(rb.position + finalDirection * patrolSpeed * Time.deltaTime);
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

        }
    }
}