using Prototype.AIProj;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Prototype.AIProj
{
    public class WaypointEvent : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {

            if (other.CompareTag("Player"))
            {
                ActionEvents.SetNextPoint?.Invoke();
            }

            if(other.CompareTag("Enemy"))
            {
                ActionEvents.SetEnemyNextPoint?.Invoke();
                Debug.LogWarning($"{ActionEvents.SetEnemyNextPoint} initiated");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
                this.gameObject.SetActive(false);
        }
    }
}

