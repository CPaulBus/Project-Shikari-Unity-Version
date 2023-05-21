using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.HighDefinition;

namespace Shikari.AI
{
    public class AIStateController : MonoBehaviour
    {
        public AIStats stats;
        public Transform eyes;

        [HideInInspector] public NavMeshAgent navMeshAgent;
        //[Insert AI Behaviour]
        [HideInInspector] public List<Transform> wayPointList;

        private bool aiActive;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public void SetupAI(bool aiActivationFromEntityManager, List<Transform> wayPointsFromMapManager)
        {
            wayPointList = wayPointsFromMapManager;
            aiActive = aiActivationFromEntityManager;

            if(aiActive)
            {
                navMeshAgent.enabled = true;
            }
            else
            {
                navMeshAgent.enabled = false;
            }
        }
    }
}
