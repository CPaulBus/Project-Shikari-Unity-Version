using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Shikari.AI
{
    public interface IAIVision
    {
        Transform GetPlayer();
    }

    public class AIVision : MonoBehaviour, IAIVision
    {
        private IEnemyMovement enemyMovement;
        private GameObject player;

        [SerializeField] private float radius;
        [SerializeField] [Range(0,360)] private float angle;
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask obstructionMask;
        [SerializeField] private bool seePlayer;
        private Transform currentTargetPosition;
        private Transform lastSeenTargetPosition;

        public Transform GetPlayer()
        {
            return player.transform;
        }

        public float Radius { get { return radius; } }

        public float Angle { get { return angle; } }
        public bool SeePlayer { get { return seePlayer; } }
        public GameObject Player { get { return player; } }

        private void Awake()
        {
            enemyMovement = GetComponentInParent<EnemyMovement>();

            angle = enemyMovement.GetCurrentStats().lookSphereCast;
            radius = enemyMovement.GetCurrentStats().lookRange;
        }

        private void Update()
        {
            StartCoroutine(FOVRoutine());

            if (seePlayer)
            {
                EnemyMovement.instance.LockOnPlayer(player.transform);
            }
            else
            {
                EnemyMovement.instance.LockOnPlayer(null);
                player = null;
            }
        }

        private void FieldOfViewCheck()
        {
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

            if (rangeChecks.Length != 0)
            {
                Transform target = rangeChecks[0].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    {
                        seePlayer = true;
                        player = rangeChecks[0].gameObject;
                    }
                    else
                    {
                        seePlayer = false;
                    }                        
                }
            }
            else if (seePlayer)
            {
                seePlayer = false;
            }
                
        }

        IEnumerator FOVRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(0.2f);

            while(true)
            {
                yield return wait;
                FieldOfViewCheck();
            }
        }
    }
}

