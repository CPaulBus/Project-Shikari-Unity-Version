using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    [RequireComponent(typeof(SphereCollider))]
    public class EnemyLineOfSightChecker : MonoBehaviour
    {
        private SphereCollider _collider;
        private float _FieldOfView;
        private LayerMask _LineOfSightLayers;

        public delegate void GainSightEvent(Player player);
        public GainSightEvent OnGainSight;
        public delegate void LoseSightEvent(Player player);
        public LoseSightEvent OnLostSight;

        #region GetSetters
        public SphereCollider Collider
        {
            get { return _collider; }
            set { _collider = value; }
        }
        public float FieldOfView
        {
            get { return _FieldOfView; }
            set { _FieldOfView = value; }
        }
        public LayerMask LineOfSightLayers
        {
            get { return _LineOfSightLayers; }
            set { _LineOfSightLayers = value; }
        }
        #endregion

        private Coroutine CheckForLineOfSightCoroutine;
        bool OnSight;

        private void Awake()
        {
            Collider=GetComponent<SphereCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            Player player;
            if (other.TryGetComponent<Player>(out player))
            {
                Debug.Log("[EnemyLineOfSightChecker] Player Detected");
                OnGainSight?.Invoke(player);
                OnSight = true;
                CheckForLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(player));
            }
        }

        private void OnTriggerStay(Collider other)
        {
            Player player;
            if (other.TryGetComponent<Player>(out player))
            {
                Debug.Log("[EnemyLineOfSightChecker] Player Detected");
                OnGainSight?.Invoke(player);
                OnSight = true;
                CheckForLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(player));
            }
        }

        

        private void OnTriggerExit(Collider other)
        {
            Player player;
            if (other.TryGetComponent<Player>(out player))
            {
                OnSight = false;
                OnLostSight?.Invoke(player);
                if (CheckForLineOfSightCoroutine != null)
                {
                    StopCoroutine(CheckForLineOfSightCoroutine);
                }
            }
        }

        private IEnumerator CheckForLineOfSight(Player player)
        {
            WaitForSeconds Wait = new WaitForSeconds(0.1f);

            while (OnSight)
            {
                yield return Wait;
            }
        }
    }
}