using System.Collections;
using System.Numerics;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts
{
    public class RangedAttackRadius : AttackRadius
    {
        private NavMeshAgent _agent;
        private Bullet _bulletPrefab;
        private Vector3 _bulletSpawnOffset = new Vector3(0, 1, 0);
        private LayerMask _mask;
        private ObjectPool _bulletPool;
        [SerializeField] private float _sphereCastRadius = 0.1f;
        public RaycastHit Hit;
        private IDamageable _targetDamageable;

        #region Get Setters
        public ObjectPool BulletPool { get { return _bulletPool; } set { _bulletPool = value; } }
        public Bullet BulletPrefab { get { return _bulletPrefab; } set { _bulletPrefab = value; } }
        public NavMeshAgent Agent { get { return _agent; } set { _agent = value; } }
        public IDamageable targetDamageable { get { return _targetDamageable; } set { _targetDamageable = value; } }
        public Vector3 BulletSpawnOffset { get { return _bulletSpawnOffset; } set { _bulletSpawnOffset = value; } }
        public float SpherecastRadius { get { return _sphereCastRadius; } set { _sphereCastRadius = value; } }
        public LayerMask Mask { get { return _mask; } set { _mask = value; } }
        #endregion

        protected override void Awake()
        {
            base.Awake();
        }

        public void CreateBulletPool()
        {
            if (BulletPool == null)
            {
                BulletPool = ObjectPool.CreateInstance(BulletPrefab, Mathf.CeilToInt((1 / AttackDelay) * BulletPrefab.AutoDestroyTime));
            }
        }

        protected override IEnumerator Attack()
        {
            WaitForSeconds Wait = new WaitForSeconds(AttackDelay);

            yield return Wait;

            while (Damageables.Count > 0)
            {
                for (int i = 0; i < Damageables.Count; i++)
                {
                    if (HasLineOfSightTo(Damageables[i].GetTransform()))
                    {
                        targetDamageable = Damageables[i];
                        OnAttack?.Invoke(Damageables[i]);
                        Agent.enabled = false;
                        break;
                    }
                }

                if (targetDamageable != null)
                {
                    PoolableObject poolableObject = BulletPool.GetObject();
                    if (poolableObject != null)
                    {
                        BulletPrefab = poolableObject.GetComponent<Bullet>();

                        BulletPrefab.Damage = Damage;
                        BulletPrefab.transform.position = transform.position + BulletSpawnOffset;
                        BulletPrefab.transform.rotation = Agent.transform.rotation;
                        BulletPrefab.Rigidbody.AddForce(Agent.transform.forward * BulletPrefab.MoveSpeed, ForceMode.VelocityChange);
                    }
                }
                else
                {
                    Agent.enabled = true; // no target in line of sight, keep trying to get closer
                }

                yield return Wait;

                if (targetDamageable == null || !HasLineOfSightTo(targetDamageable.GetTransform()))
                {
                    Agent.enabled = true;
                }

                Damageables.RemoveAll(DisabledDamageables);
            }

            Agent.enabled = true;
            AttackCoroutine = null;
        }

        private bool HasLineOfSightTo(Transform Target)
        {
            if (Physics.SphereCast(transform.position + BulletSpawnOffset, SpherecastRadius, ((Target.position + BulletSpawnOffset) - (transform.position + BulletSpawnOffset)).normalized, out Hit, Collider.radius, Mask))
            {
                IDamageable damageable;
                if (Hit.collider.TryGetComponent<IDamageable>(out damageable))
                {
                    return damageable.GetTransform() == Target;
                }
            }

            return false;
        }

        protected override void OnTriggerExit(Collider other)
        {
            base.OnTriggerExit(other);

            if (AttackCoroutine == null)
            {
                Agent.enabled = true;
            }
        }
    }
}