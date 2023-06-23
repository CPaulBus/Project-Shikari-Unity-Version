using UnityEngine;
using Assets.Scripts;

namespace Assets.Scripts.Enemy
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class EnemyPainResponse : MonoBehaviour, IPainResponse
    {
        #region Auto Initialization
        private EnemyHealth Health;
        private Animator Animator;
        #endregion

        [SerializeField][Range(1, 100)] private int MaxDamagePainTreshold = 5;

        private const string DEATH_TRIGGER = "Die";
        private const string HIT_TRIGGER = "Hit";

        void Awake()
        {
            Health = GetComponent<EnemyHealth>();
            Animator = GetComponent<Animator>();
        }


        public void HandleDeath()
        {
            Animator.applyRootMotion = true;
            Animator.SetTrigger(DEATH_TRIGGER);
        }

        public void HandlePain(int Damage)
        {
            if (Health.CurrentHealth != 0)
            {
                Animator.ResetTrigger(HIT_TRIGGER);
                Animator.SetLayerWeight(2, (float)Damage / MaxDamagePainTreshold);
                Animator.SetTrigger(HIT_TRIGGER);
            }
        }
    }
}