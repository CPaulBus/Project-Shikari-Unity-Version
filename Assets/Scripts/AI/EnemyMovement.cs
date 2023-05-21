using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Shikari.AI;
using JetBrains.Annotations;
using System;

namespace Shikari.AI
{
    public interface IEnemyMovement
    {
        AIStats GetCurrentStats();
    }

    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyMovement : MonoBehaviour, IEnemyMovement
    {
        public float UpdateSpeed = 0.1f;

        [SerializeField] private AIStats enemyAIStats;

        private NavMeshAgent _agent;

        public Action<Transform> LockOnPlayer;

        public static EnemyMovement instance;

        public AIStats GetCurrentStats()
        {
            return enemyAIStats;
        }

        private void Awake()
        {
            instance = this;

            _agent = GetComponent<NavMeshAgent>();

            _agent.speed = enemyAIStats.speed;
            _agent.stoppingDistance = enemyAIStats.AttackRange;
        }

        void OnEnable()
        {
            LockOnPlayer += StartFollowingTarget;
        }

        private void Update()
        {
            
        }
        void StartFollowingTarget(Transform Target)
        {
            if (Target != null)
            {
                StartCoroutine(FollowTarget(Target));
            }
        }

        IEnumerator FollowTarget(Transform Target)
        {
            WaitForSeconds Wait = new WaitForSeconds(UpdateSpeed);

            while (enabled)
            {
                _agent.SetDestination(Target.position);

                yield return Wait;
            }
        }
    }
}
