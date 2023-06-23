using UnityEngine;

namespace Assets.Scripts.Enemy
{
    [DisallowMultipleComponent]
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [Header("Auto-Initialization on Runtime via Scriptable Object")]
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _currentHealth;

        public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }

        public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }

        public event IDamageable.TakeDamageEvent OnTakeDamage;
        public event IDamageable.DeathEvent OnDeath;

        public Transform GetTransform()
        {
            return transform;
        }

        public void TakeDamage(int Damage)
        {
            int damageTaken = Mathf.Clamp(Damage, 0, CurrentHealth);

            CurrentHealth -= damageTaken;

            if (damageTaken != 0)
            {
                OnTakeDamage?.Invoke(damageTaken);
            }

            if (CurrentHealth == 0 && damageTaken != 0)
            {
                OnDeath?.Invoke(transform.position);
            }
        }
    }
}