using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemy
{
    [RequireComponent(typeof(Animator), typeof(EnemyMovement), typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyHealth), typeof(EnemyPainResponse), typeof(EnemyExpression))]
    public class Enemy : PoolableObject
    {
        /// <summary>
        /// This is the Main hub for Enemy Script
        /// Put this Enemy Script to the Enemy GameObject.
        /// </summary>
        /// 
        #region Enemy Scriptable Object
        [SerializeField] private EnemyScriptableObject _enemyScriptableObject;
        #endregion

        #region Auto Initialization at Runtime        
        private Animator _animator;
        [Header("Auto Initialization at Runtime")]
        public EnemyMovement Movement;
        public EnemyHealth Health;
        public EnemyPainResponse PainResponse;
        public NavMeshAgent Agent;
        #endregion

        #region Manual Initialization
        [Header("Manual Initialization")]
        [SerializeField] private AttackRadius _attackRadius;

        #region GetSetters
        public AttackRadius AttackRadius { get { return _attackRadius; } set { _attackRadius = value; } }
        #endregion

        #endregion

        private Coroutine LookCoroutine;
        public const string ATTACK_TRIGGER = "Attack";

        private bool firstTimeSetup = false;

        void Awake()
        {
            AutoComponentInit();

            void AutoComponentInit()
            {
                _animator = GetComponent<Animator>();
                Movement = GetComponent<EnemyMovement>();
                Agent = GetComponent<NavMeshAgent>();
                Health = GetComponent<EnemyHealth>();
                PainResponse = GetComponent<EnemyPainResponse>();
            }
        }

        private void EventAssignments(bool eventAssigning)
        {
            if (eventAssigning)
            {
                Movement.SceneSpawnedEvent += SetupEnemy;
                Health.OnTakeDamage += OnHit;
                Health.OnDeath += Die;
                _attackRadius.OnAttack += OnAttack;
                _animator.fireEvents = true;
            }
            else{
                Movement.SceneSpawnedEvent -= SetupEnemy;
                Health.OnTakeDamage -= OnHit;
                Health.OnDeath -= Die;
                _attackRadius.OnAttack -= OnAttack;
                _animator.fireEvents = false;
            }

        }

        void OnEnable()
        {
            EventAssignments(true);

            if (firstTimeSetup)
            {
                Movement.StartMoving();
                _attackRadius.gameObject.SetActive(true);
                this.GetComponent<Collider>().enabled = true;
            }

            firstTimeSetup = true;
        }

        public override void OnDisable()
        {
            base.OnDisable();

            Agent.enabled = false;
            EventAssignments(false);
        }
        void SetupEnemy()
        {
            Agent.enabled = true;
            _enemyScriptableObject.SetupEnemy(this);
        }

        void OnAttack(IDamageable Target)
        {
            _animator.SetTrigger(ATTACK_TRIGGER);

            if (LookCoroutine != null)
            {
                StopCoroutine(LookCoroutine);
            }

            LookCoroutine = StartCoroutine(LookAt(Target.GetTransform()));
        }

        void Die(Vector3 Position)
        {
            Movement.StopMoving(); //Stop the movement.            
            PainResponse.HandleDeath();
            _attackRadius.gameObject.SetActive(false);
            this.GetComponent<Collider>().enabled = false;
        }

        void OnHit(int Damage)
        {
            PainResponse.HandlePain(Damage);
        }

        private IEnumerator LookAt(Transform Target)
        {
            Quaternion lookRotation = Quaternion.LookRotation(Target.position - transform.position);
            float time = 0;

            while (time < 1)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);

                time += Time.deltaTime * 2;
                yield return null;
            }

            transform.rotation = lookRotation;
        }

        public void BodyDisappear(){
            StartCoroutine(StartDisableEntity());
        }

        IEnumerator StartDisableEntity()
        {
            yield return new WaitForSeconds(3f);
            this.gameObject.SetActive(false);
        }
    }
}