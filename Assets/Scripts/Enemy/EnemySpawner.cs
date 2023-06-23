using Prototype.AIProj.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        public Transform Player;
        public int NumberOfEnemiesToSpawn = 5;
        public float SpawnDelay = 1f;
        public List<EnemyScriptableObject> Enemies = new List<EnemyScriptableObject>();
        public SpawnMethod EnemySpawnMethod = SpawnMethod.RoundRobin;

        private NavMeshTriangulation Triangulation;
        private Dictionary<int, ObjectPool> EnemyObjectPools = new Dictionary<int, ObjectPool>();

        private void Initialization()
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;

            for (int i = 0; i < Enemies.Count; i++)
            {
                EnemyObjectPools.Add(i, ObjectPool.CreateInstance(Enemies[i].Prefab, NumberOfEnemiesToSpawn));
            }
        }

        private void Start()
        {
            Initialization();
            Triangulation = NavMesh.CalculateTriangulation();

            StartCoroutine(SpawnEnemies());
        }

        void LateUpdate(){
            if(GameObject.FindGameObjectsWithTag("Enemy").Length <=0)
                StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            WaitForSeconds Wait = new WaitForSeconds(SpawnDelay);

            int SpawnedEnemies = 0;

            while (SpawnedEnemies < NumberOfEnemiesToSpawn)
            {
                switch (EnemySpawnMethod)
                {
                    case SpawnMethod.RoundRobin:
                        SpawnRoundRobinEnemy(SpawnedEnemies);
                        break;

                    case SpawnMethod.Random:
                        SpawnRandomEnemy();
                        break;
                }

                SpawnedEnemies++;

                yield return Wait;
            }
        }

        private void SpawnRoundRobinEnemy(int SpawnedEnemies)
        {
            int SpawnIndex = SpawnedEnemies % Enemies.Count;

            DoSpawnEnemy(SpawnIndex);
        }

        void SpawnRandomEnemy()
        {
            DoSpawnEnemy(Random.Range(0, Enemies.Count));
        }

        void DoSpawnEnemy(int SpawnIndex)
        {
            PoolableObject poolableObject = EnemyObjectPools[SpawnIndex].GetObject();

            if (poolableObject != null)
            {
                Enemy enemy = poolableObject.GetComponent<Enemy>();
                Enemies[SpawnIndex].SetupEnemy(enemy);

                int VertexIndex = Random.Range(0, Triangulation.vertices.Length);

                NavMeshHit Hit;
                if (NavMesh.SamplePosition(Triangulation.vertices[VertexIndex], out Hit, 2f, -1))
                {
                    enemy.Agent.Warp(Hit.position);
                    if (enemy.Movement.DefaultState == EnemyState.Chase)
                        enemy.Movement.Target = Player;
                    enemy.Movement.Triangulation = Triangulation;
                    enemy.Agent.enabled = true;
                    enemy.Movement.Spawn();
                }
                else
                {
                    Debug.LogError($"Unable to place NavMeshAgent on NavMesh. Tried to use {Triangulation.vertices[VertexIndex]}");
                }
            }
            else
            {
                Debug.LogError($"Unable to fetch enemy of type {SpawnIndex} from object pool. Out of objects?");
            }
        }
    }
}