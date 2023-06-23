using UnityEngine;

public class SpawnParticleSystemOnDeath : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem DeathSystem;
    public IDamageable Damageable;

    private void Awake()
    {
        Damageable = GetComponentInParent<IDamageable>();
    }

    private void OnEnable()
    {
        Damageable.OnDeath += Damageable_OnDeath;
    }

    private void Damageable_OnDeath(Vector3 Position)
    {
        Instantiate(DeathSystem, this.transform.position, Quaternion.identity);
    }

}