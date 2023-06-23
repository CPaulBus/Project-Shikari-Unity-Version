using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.AIProj
{
    public class WaypointManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> WayPoints;

        public static WaypointManager instance;
        public int currentNum;
        public int enemyCurrentNum;
        public bool isEnemyWaypoint;

        public GameObject GetWaypoint
        {
            get
            {
                if (!isEnemyWaypoint)
                {
                    if (currentNum < WayPoints.Count)
                    {
                        var WaypointSelected = WayPoints[currentNum];
                        WaypointSelected.SetActive(true);
                        currentNum++;
                        return WaypointSelected;
                    }
                    else
                    {
                        currentNum = 1;
                        WayPoints[0].SetActive(true);
                        return WayPoints[0];
                    }
                }
                else
                {
                    if (currentNum < WayPoints.Count)
                    {
                        var WaypointSelected = WayPoints[currentNum];
                        WaypointSelected.SetActive(true);
                        enemyCurrentNum++;
                        return WaypointSelected;
                    }
                    else
                    {
                        enemyCurrentNum = 1;
                        WayPoints[0].SetActive(true);
                        return WayPoints[0];
                    }
                }
            }
        }

        public GameObject GetStarterWaypoint
        {
            get
            {
                WayPoints[0].SetActive(true);
                return WayPoints[0];
            }
        }

        private void Awake()
        {
            instance = this;
            currentNum++;
            for (int i = 1; i < WayPoints.Count; i++)
            {
                WayPoints[i].SetActive(false);
            }
        }

        private void Start()
        {
            
        }
    }
}

