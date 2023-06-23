using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    [RequireComponent(typeof(EnemyHealth))]
    public class EnemyExpression : MonoBehaviour
    {
        [Tooltip("Put Face Object Here")]
        [SerializeField] SkinnedMeshRenderer characterFace;

        #region External Scripts
        EnemyHealth Health;
        EnemyMovement Movement;
        #endregion

        public SkinnedMeshRenderer CharacterFace { get => characterFace; }

        void Awake()
        {
            Health = GetComponent<EnemyHealth>();
        }

        void OnEnable()
        {
            EventsAssignments(true);
        }

        void OnDisable()
        {
            EventsAssignments(false);
            Reset();
        }

        void EventsAssignments(bool OnEventStart)
        {
            if (OnEventStart)
            {
                Health.OnDeath += OnDeathExpression;
                Health.OnTakeDamage += OnHitExpression;
            }
            else
            {
                Health.OnDeath -= OnDeathExpression;
                Health.OnTakeDamage -= OnHitExpression;
            }
        }

        private void OnDeathExpression(Vector3 Position)
        {
            Debug.Log("[EnemyExpression.cs] Death Expression Initiated");
            CharacterFace.SetBlendShapeWeight(13, 100);
        }

        private void OnHitExpression(int Damage)
        {
            Debug.Log("[EnemyExpression.cs] Hit Expression Initiated");
            CharacterFace.SetBlendShapeWeight(1, 100);
        }

        public void Reset()
        {
            if (CharacterFace.GetBlendShapeWeight(13) > 0)
                CharacterFace.SetBlendShapeWeight(13, 0);

            if (CharacterFace.GetBlendShapeWeight(1) > 0)
                CharacterFace.SetBlendShapeWeight(1, 0);
        }

        IEnumerator ExpressionDelay(){
            WaitForSeconds wait = new WaitForSeconds(0.1f);

            yield return wait;
            Reset();
        }
    }

}