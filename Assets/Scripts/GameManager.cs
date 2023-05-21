using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private int WavesSurvived = 0;
    [SerializeField] private List<GameObject> playerSpawn;
    [SerializeField] List<PlayerCharacterSO> characterPrefabs;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if(playerSpawn.Count >= 0)
        {
            GameObject[] playerSpawnsGO = GameObject.FindGameObjectsWithTag("PlayerSpawn");

            foreach(GameObject playerSpawnpt in playerSpawnsGO)
            {
                playerSpawn.Add(playerSpawnpt);
            }
        }

        characterPrefabs[0].Spawn(playerSpawn[0].transform, this);
    }

    public void AddWavesSurvived()
    {
        WavesSurvived++;
    }

    public void GameOver()
    {
        Debug.Log("[GameOver] Moving to the escaping scene.");
    }
}
