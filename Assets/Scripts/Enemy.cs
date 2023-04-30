using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace KUNAI.Enemy {
    public class Enemy : MonoBehaviour
    {
        EnemyManager enemyManager;
        public Rigidbody rb;
        public NavMeshAgent agent;
        private Animator eAnimator;

        public GameObject projectilePrefab;
        public float spawnDistance;

        public float fireRate;
        float fireTimer;

        

        public enum AIState {
            Engaging,
            Hunting,
            Investigating
        }

        public AIState state;

        [Header("AI")]
        public float investigationRange = 10;
        Vector3 walkPoint;
        bool walkPointSet;
        public LayerMask groundLayers;

        [Header("Detection")]
        public float sightRadius;
        [Range(0, 360)]
        public float sightAngle;
        
        public LayerMask targetLayers;
        public LayerMask obstructionLayers;

        public bool canSeePlayer;
        float sightDelay = 0.05f;

        void Start()
        {
            eAnimator = GetComponent<Animator>(); 
            enemyManager = EnemyManager.GetInstance();

            rb = GetComponent<Rigidbody>();
            agent = GetComponent<NavMeshAgent>();

            //StartCoroutine(FOVRoutine());

            state = AIState.Investigating;
        }

        void Update() {
            CheckFOV();

            fireTimer -= Time.deltaTime;
            
            if (canSeePlayer) {
                state = AIState.Engaging;
            }

            switch (state) {
                case AIState.Engaging:
                    HandleEngaging();
                    break;
                case AIState.Hunting:
                    HandleHunting();
                    break;
                case AIState.Investigating:
                    HandleInvestigating();
                    break;
            }
            

            
            
        }

        void FixedUpdate()
        {
            //rb.velocity = (transform.position - enemyManager.player.transform.position).normalized * -10;
        }

        void Throw(Quaternion angle) {
            eAnimator.SetTrigger("trThrow");
            eAnimator.SetBool("isHunting", false);
            GameObject proj = Instantiate(projectilePrefab);
            proj.transform.position = transform.position + (transform.forward * spawnDistance) + (transform.up * 1);

            proj.transform.rotation = angle;

        }

        /*private IEnumerator FOVRoutine() {
            WaitForSeconds wait = new WaitForSeconds(sightDelay);

            while(true) {
                yield return wait;
                CheckFOV();
            }
        }*/

        void CheckFOV() {
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, sightRadius, targetLayers);

            if (rangeChecks.Length != 0) {
                Transform target = rangeChecks[0].transform;
                Vector3 targetDirection = (target.position - transform.position).normalized;

                if(Vector3.Angle(transform.forward, targetDirection) < sightAngle / 2) {
                    float targetDistance = Vector3.Distance(transform.position, target.position);

                    if(!Physics.Raycast(transform.position, targetDirection, targetDistance, obstructionLayers)) {
                        canSeePlayer = true;
                        enemyManager.lastKnownLocation = target.position;
                    } else {
                        canSeePlayer = false;
                    }
                } else {
                    canSeePlayer = false;
                }
            } else if (canSeePlayer) canSeePlayer = false;
        }

        void HandleEngaging() {
            if (!canSeePlayer) {
            eAnimator.SetBool("isHunting", true);
            eAnimator.SetBool("isInvestigating", false);
                state = AIState.Hunting;
                return;
            }
            Vector3 playerDirection = (enemyManager.player.transform.position - transform.position).normalized;
            Quaternion playerAngle = Quaternion.LookRotation(playerDirection);

            Vector3 eulerRot = transform.rotation.eulerAngles;

            transform.rotation = Quaternion.Euler(eulerRot.x, playerAngle.eulerAngles.y, eulerRot.z);

            if (fireTimer <= 0) {
                Throw(playerAngle);
                fireTimer = fireRate;
            }

            agent.SetDestination(transform.position);
        }

        void HandleHunting() {
            agent.SetDestination(enemyManager.lastKnownLocation);
            float dist = Vector3.Distance(transform.position, enemyManager.lastKnownLocation);

            if (dist < 0.1) 
            {
                state = AIState.Investigating;
                eAnimator.SetBool("isHunting", false);
                eAnimator.SetBool("isInvestigating", true);
            }
        }

        void HandleInvestigating() {
            if (!walkPointSet) SearchPoint();
                else agent.SetDestination(walkPoint);

            float dist = Vector3.Distance(transform.position, walkPoint);

            if (dist < 0.1) walkPointSet = false;
        }

        void SearchPoint() {
            float randomZ = Random.Range(-investigationRange, investigationRange);
            float randomX = Random.Range(-investigationRange, investigationRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayers)) walkPointSet = true;
        }

        void OnTriggerEnter(Collider other) {
            Debug.Log("kill me");
            if (other.GetComponent<Hitbox>() != null) {
                Destroy(gameObject);
            }
        }
    }
}
