using System;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    [CreateAssetMenu(fileName = "Enemy Attack Scriptable Object", menuName = "ScriptableObject/Attack Configuration", order = 1)]
    public class AttackScriptableObject: ScriptableObject
    {
        public int Damage = 5;
        public float AttackRadius = 1.5f;
        public float AttackDelay = 1.5f;

        [Header("Ranged Configuration")]
        public bool isRanged = false;
        public Bullet BulletPrefab;
        public Vector3 BulletSpawnOffset = new Vector3(0,1,0);
        public LayerMask LineOfSightLayers;

        public void SetupEnemy(Enemy enemy)
        {
            (enemy.AttackRadius.Collider == null ? enemy.AttackRadius.GetComponent<SphereCollider>() : enemy.AttackRadius.Collider).radius = AttackRadius;
            enemy.AttackRadius.AttackDelay = AttackDelay;
            enemy.AttackRadius.Damage = Damage;
            enemy.Movement.LineOfSightChecker.LineOfSightLayers = LineOfSightLayers;

            if (isRanged)
            {
                RangedAttackRadius rangedAttackRadius = enemy.AttackRadius.GetComponent<RangedAttackRadius>();

                rangedAttackRadius.BulletPrefab = BulletPrefab;
                rangedAttackRadius.BulletSpawnOffset = BulletSpawnOffset;
                rangedAttackRadius.Mask = LineOfSightLayers;

                rangedAttackRadius.CreateBulletPool();
            }
        }
    }
}