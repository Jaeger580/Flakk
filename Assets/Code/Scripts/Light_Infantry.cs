using System.Collections;
using System.Collections.Generic;
using GeneralUtility;
using GeneralUtility.EditorQoL;
using UnityEngine;

namespace JO.AI 
{
    abstract public class EnemyAI : AI_FSM, IDamagable
    {
        protected Rigidbody rb;

        [Header("Settings")]
        [SerializeField] protected int health;
        [SerializeField] protected int damageOutput;
        [SerializeField] protected List<Transform> patrolPoints = new List<Transform>();
        [SerializeField] protected int currentPatrolIndex;
        [SerializeField] protected WebSelection currentWeb;
        protected float speed;
        [SerializeField] protected float constSpeed;
        protected bool isbraking = false;
        [SerializeField] protected float brake;
        [SerializeField] protected float rotationSpeed;
        [SerializeField] protected float minimumDistance = .1f;

        [SerializeField] protected Transform target;
        [SerializeField] protected LayerMask ignoreLayer;
        [SerializeField] protected float obstacleForwardDetectionDistance;
        [SerializeField] protected float obstacleUpDetectionDistance;
        [SerializeField] protected float obstacleDownDetectionDistance;
        [SerializeField] protected float obstacleLeftDetectionDistance;
        [SerializeField] protected float obstacleRightDetectionDistance;
        [SerializeField] protected float avoidForce;
        [SerializeField] protected List<Transform> targetList;
        [SerializeField] protected int currentTargetIndex;
        protected float targetDistance;
        [SerializeField] protected AI_STATE currentState;

        [SerializeField] protected float fireRate;
        protected bool isFiring = false;

        [Header("Aggro Timer")]
        [SerializeField] protected float aggroCooldown;
        [ReadOnly] [SerializeField] protected float aggroTimer;
        [ReadOnly] [SerializeField] protected bool recentlyAggro = false;

        public delegate void OnEnemyDeath();
        public OnEnemyDeath EnemyDeathEvent;

        abstract public void TakeDamage(int _damage);

        abstract protected void Fire();

        virtual protected void Death()
        {
            EnemyDeathEvent?.Invoke();
            EnemyDeathEvent = null;
            Destroy(gameObject);
        }

        public void SetChosenWeb(WebSelection chosenWeb)
        {
            currentWeb = chosenWeb;
        }

        protected Transform FindNearest(ref List<Transform> points, float minDist)
        {
            if (points.Count <= 0) { Editor_Utility.ThrowWarning("ERR: No points in list, can't find nearest.", this); return null; }
            float distToPoint = 100000000f;
            var transPos = transform.position;
            Transform nearestPoint = points[0];
            List<Transform> tempPoints = new();

            foreach (var point in points)
            {
                tempPoints.Add(point);
            }

            foreach (var p in tempPoints)
            {
                if (p == null) points.Remove(p);
            }

            foreach (var possiblePoint in points)
            {//Look for the closest waypoint
                var testDist = Vector3.Distance(transPos, possiblePoint.position);
                if (testDist <= minDist) continue;
                if (testDist < distToPoint)
                {
                    distToPoint = testDist;
                    nearestPoint = possiblePoint;
                    //print($"Distance to {nearestPoint.name} is {distToPoint}");
                }
            }
            //Editor_Utility.ThrowWarning($"Settled on {nearestPoint.name}", nearestPoint);
            return nearestPoint;
        }

        protected bool Retarget()
        {
            float bestTurn = 45;
            foreach(var weakpoint in targetList)
            {
                var angle = Vector3.Angle(transform.forward, (weakpoint.position - transform.position).normalized);
                if (angle < bestTurn)
                {
                    bestTurn = angle;
                    currentTargetIndex = targetList.IndexOf(weakpoint);
                    return true;
                }
            }

            return false;
        }

        protected void Aggro()
        {
            if (!targetList.Contains(target) || target == null)
            {
                if (Retarget())
                    target = targetList[currentTargetIndex];
                else
                {
                    currentState = AI_STATE.PATROL;
                    return;
                }
            }

            targetDistance = Vector3.Distance(transform.position, target.position);
            //var targetAngle = Vector3.Angle(transform.forward, transform.position - target.position);

            if (targetDistance <= 45f)
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

        protected void MoveShip()
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Vector3 avoidanceDirection = Vector3.zero;

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, obstacleForwardDetectionDistance))
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
    }

    [RequireComponent(typeof(Rigidbody))]
    public class Light_Infantry : EnemyAI
    {
        public Transform left_Fire_Point, right_Fire_Point;
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

        private void Update()
        {
            if (recentlyAggro)
            {//if I was recently in the aggro state, add to my timer
                aggroTimer += Time.deltaTime;
            }

            if (aggroTimer >= aggroCooldown)
            {//if my timer exceeds my cooldown, reset my aggro status
                recentlyAggro = false;
                aggroTimer = 0f;
            }
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
            bool isFireLeft = true;

            while(currentState == AI_STATE.AGGRO)
            {
                isFiring = true;

                GameObject projectile = null;

                if (isFireLeft)
                {
                    projectile = Instantiate(projectilePrefab, left_Fire_Point.position, left_Fire_Point.rotation);
                    isFireLeft = false;
                }
                else
                {
                    projectile = Instantiate(projectilePrefab, right_Fire_Point.position, left_Fire_Point.rotation);
                    isFireLeft = true;
                }

                if (projectile.GetComponent<Missile>())
                {
                    projectile.GetComponent<Missile>().Fire(target, damageOutput);
                }

                yield return new WaitForSeconds(fireRate);
            }

            isFiring = false;
        }

        override public void TakeDamage(int _damage)
        {
            health -= _damage;

            if(health <= 0)
            {
                AudioManager.instance.Play("Distant Exp");
                Death();
            }
        }
    }
}