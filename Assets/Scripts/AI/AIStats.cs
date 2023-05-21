using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shikari.AI
{
    [CreateAssetMenu(menuName = "AI/AI Stats",fileName = "DefaultAIStats")]
    public class AIStats : ScriptableObject
    {
        public float maxHealth = 100f;

        public float maxShield = 100f;

        public float damage = 1f;
        public float speed = 2f;
        public float lookRange = 40f;
        public float lookSphereCast = 1f;
        public float searchDuration = 4f;
        public float searchingTurnSpeed = 1f;

        [Header("BattleStats")]
        public float AttackRange = 5f;
        public float AttackCooldown = 1f;
        public float AttackSpeed = 1f;
    }
}

