using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(SphereCollider))]
    public class AttackRadius : MonoBehaviour
    {
        private SphereCollider _collider;
        protected List<IDamageable> Damageables = new List<IDamageable>();
        private int _damage = 10;
        private float _attackDelay = 0.5f;

        #region Events
        public delegate void AttackEvent(IDamageable Target);
        public AttackEvent OnAttack;
        #endregion

        #region Public Get Setter Variables
        public SphereCollider Collider { get { return _collider; } set { _collider = value; } }
        public int Damage { get { return _damage; } set { _damage = value; } }
        public float AttackDelay { get { return _attackDelay; } set { _attackDelay = value; } }
        #endregion

        protected Coroutine AttackCoroutine;

        protected virtual void Awake()
        {
            Collider = GetComponent<SphereCollider>();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Damageables.Add(damageable);

                if (AttackCoroutine == null)
                {
                    AttackCoroutine = StartCoroutine(Attack());
                }
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Damageables.Remove(damageable);
                if (Damageables.Count == 0)
                {
                    StopCoroutine(AttackCoroutine);
                    AttackCoroutine = null;
                }
            }
        }

        protected virtual IEnumerator Attack()
        {
            WaitForSeconds Wait = new WaitForSeconds(AttackDelay);

            yield return Wait;

            IDamageable closestDamageable = null;
            float closestDistance = float.MaxValue;

            while (Damageables.Count > 0)
            {
                for (int i = 0; i < Damageables.Count; i++)
                {
                    Transform damageableTransform = Damageables[i].GetTransform();
                    float distance = Vector3.Distance(transform.position, damageableTransform.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestDamageable = Damageables[i];
                    }
                }

                if (closestDamageable != null)
                {
                    OnAttack?.Invoke(closestDamageable);
                    closestDamageable.TakeDamage(Damage);
                }

                closestDamageable = null;
                closestDistance = float.MaxValue;

                yield return Wait;

                Damageables.RemoveAll(DisabledDamageables);
            }

            AttackCoroutine = null;
        }

        protected bool DisabledDamageables(IDamageable Damageable)
        {
            return Damageable != null && !Damageable.GetTransform().gameObject.activeSelf;
        }
    }
}