using Prototype.AIProj;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.HighDefinition;

namespace Assets.Scripts.Enemy
{
    public class EnemyMovement : MonoBehaviour
    {
        public EnemyLineOfSightChecker LineOfSightChecker;
        [SerializeField] private Transform currentTarget;
        public NavMeshTriangulation Triangulation;
        [SerializeField] private float _updateRate = 0.1f;
        [SerializeField] private bool onTheSceneSpawned = false;

        public float UpdateRate
        {
            get { return _updateRate; }
            set { _updateRate = value; }
        }

        public Transform Target { get=>currentTarget; set=>currentTarget=value;}

        #region Auto Init
        private NavMeshAgent Agent;
        private AgentLinkMover LinkMover;
        private Animator Animator = null;
        #endregion

        [Header("Enemy States")]
        public EnemyState DefaultState;
        private EnemyState _state;
        public EnemyState State
        {
            get { return _state; }
            set
            {
                OnStateChange?.Invoke(_state, value);
                _state = value;
            }
        }
        public float IdleLocationRadius = 4f;
        public float IdleMoveSpeedMultiplier = 0.5f;
        [SerializeField] private int WaypointIndex = 0;
        public Vector3[] Waypoints;

        #region Events
        public delegate void StateChangeEvent(EnemyState oldState, EnemyState newState);
        public delegate void OnTheSceneSpawned();
        public OnTheSceneSpawned SceneSpawnedEvent;
        public StateChangeEvent OnStateChange;

        #endregion

        #region Constant Strings for Animation
        private const string IsWalking = "isMoving";
        private const string Jump = "Jump";
        private const string Landed = "Landed";
        #endregion

        private Coroutine FollowCoroutine;

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            LinkMover = GetComponent<AgentLinkMover>();
            Animator = GetComponent<Animator>();

            LinkMover.OnLinkStart += HandleLinkStart;
            LinkMover.OnLinkEnd += HandleLinkEnd;

            OnStateChange += HandleStateChange;

            LineOfSightChecker.OnGainSight += GainSight;
            LineOfSightChecker.OnLostSight += LostSight;
        }

        private void Start()
        {
            if (onTheSceneSpawned)
            {
                SceneSpawnedEvent?.Invoke();
                Spawn();
            }
        }

        private void OnDisable()
        {
            _state = DefaultState;
        }

        void GainSight(Player player)
        {
            if (DefaultState != EnemyState.Chase)
            {
                currentTarget = currentTarget == null ? player.transform : null;
            }

            State = EnemyState.Chase;
        }

        void LostSight(Player player)
        {
            State = DefaultState;
            if (currentTarget != null && DefaultState != EnemyState.Chase)
                currentTarget = null;
        }

        public void Spawn()
        {
            #region WAYPOINT INITIALIZATION

            for (int i = 0; i < 4; i++)
            {
                NavMeshHit Hit;
                if (NavMesh.SamplePosition(Triangulation.vertices[Random.Range(0, Triangulation.vertices.Length)], out Hit, 2f, Agent.areaMask))
                {
                    Waypoints[i] = Hit.position;
                }
                else
                {
                    Debug.LogError("Unable to find position for navmesh near Triangulation vertex!");
                }
            }
            #endregion

            OnStateChange?.Invoke(EnemyState.Spawn, DefaultState);
        }

        private void HandleLinkStart()
        {
            Animator.SetTrigger(Jump);
        }

        private void HandleLinkEnd()
        {
            Animator.SetTrigger(Landed);
        }

        private void Update()
        {
            Animator.SetBool(IsWalking, Agent.velocity.magnitude > 0.01f);
        }

        private void HandleStateChange(EnemyState oldState, EnemyState newState)
        {
            if (oldState != newState)
            {
                if (FollowCoroutine != null)
                {
                    StopCoroutine(FollowCoroutine);
                }

                if (oldState == EnemyState.Idle)
                {
                    Agent.speed /= IdleMoveSpeedMultiplier;
                }

                switch (newState)
                {
                    case EnemyState.Idle:
                        FollowCoroutine = StartCoroutine(DoIdleMotion());
                        break;

                    case EnemyState.Patrol:
                        FollowCoroutine = StartCoroutine(DoPatrolMotion());
                        break;

                    case EnemyState.Chase:
                        FollowCoroutine = StartCoroutine(FollowTarget());
                        break;
                }
            }

        }

        private IEnumerator DoPatrolMotion()
        {
            WaitForSeconds Wait = new WaitForSeconds(_updateRate);

            while (true)
            {
                if (!Agent.enabled || !Agent.isOnNavMesh)
                {
                    yield return Wait;
                }
                else if (Agent.remainingDistance <= Agent.stoppingDistance)
                {
                    WaypointIndex++;

                    if (WaypointIndex >= Waypoints.Length)
                    {
                        WaypointIndex = 0;
                    }

                    Agent.SetDestination(Waypoints[WaypointIndex]);
                }

                yield return Wait;
            }
        }

        private IEnumerator DoIdleMotion()
        {
            WaitForSeconds Wait = new WaitForSeconds(_updateRate);

            Agent.speed *= IdleMoveSpeedMultiplier;

            while (true)
            {
                if (!Agent.enabled || !Agent.isOnNavMesh)
                {
                    yield return Wait;
                }
                else if (Agent.remainingDistance <= Agent.stoppingDistance)
                {
                    Vector2 point = UnityEngine.Random.insideUnitCircle * IdleLocationRadius;
                    NavMeshHit hit;

                    if (NavMesh.SamplePosition(Agent.transform.position + new Vector3(point.x, 0, point.y), out hit, 2f, Agent.areaMask))
                    {
                        Agent.SetDestination(hit.position);
                    }
                }

                yield return Wait;
            }
        }

        private IEnumerator FollowTarget()
        {
            WaitForSeconds Wait = new WaitForSeconds(_updateRate);

            Debug.Log($"{this.gameObject.name} started chasing the player");

            while (true)
            {
                if (Agent.enabled)
                {
                    if (currentTarget != null)
                        Agent.SetDestination(currentTarget.transform.position);
                }
                yield return Wait;
            }
        }

        public void StopMoving()
        {
            StopAllCoroutines();
            Agent.enabled = false;
        }

        public void StartMoving()
        {
            Agent.enabled = true;
        }

        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < Waypoints.Length; i++)
            {
                Gizmos.DrawWireSphere(Waypoints[i], 0.25f);
                if (i + 1 < Waypoints.Length)
                {
                    Gizmos.DrawLine(Waypoints[i], Waypoints[i + 1]);
                }
                else
                {
                    Gizmos.DrawLine(Waypoints[i], Waypoints[0]);
                }
            }
        }
    }
}