using Prototype.AIProj;
using System;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField]
    private Animator Animator;

    [SerializeField]
    private int Health = 300;
    private int _MaxHealth = 300;

    public bool DEBUG_MODE = false;

    public event IDamageable.TakeDamageEvent OnTakeDamage;
    public event IDamageable.DeathEvent OnDeath;

    public int CurrentHealth
    {
        get
        {
            return Health;
        }
        set
        {
            Health = value;
        }
    }

    public int MaxHealth
    {
        get { return _MaxHealth; }
        set { _MaxHealth = value; }
    }

    void OnEnable()
    {
        OnTakeDamage += TakeDamage;
    }

    public void TakeDamage(int Damage)
    {
        if (!DEBUG_MODE)
            TakingDamage(Damage);

        Debug.Log($"{this.name} is taking damage");

        void TakingDamage(int Damage)
        {
            Health -= Damage;

            if (CurrentHealth <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}