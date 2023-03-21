using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;
    private Rigidbody bulletRigidbody;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        bulletRigidbody.AddForce(transform.forward * _speed, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bullet"))
            Destroy(gameObject);

        //if (other.tag != "Player" || _playerID != _bulletID)
        //{
        //    // Hit something else
        //    //Instantiate(vfxHitRed, transform.position, Quaternion.identity);
        //    Destroy(gameObject);
        //}
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Bullet"))
            Destroy(gameObject);
    }
}
