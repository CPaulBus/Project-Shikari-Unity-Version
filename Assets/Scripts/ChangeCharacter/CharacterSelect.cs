using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    public CharSelect CurrCharSelect;

    [SerializeField] private List<CinemachineVirtualCamera> cinemachineCam;

    [SerializeField] List<PlayerCharacterSO> characterPrefabs;

    private void Start()
    {
        PlayerCharacterSO player = characterPrefabs.Find(player => CurrCharSelect == player.characterType);

        player.Spawn(this.transform, this);

        foreach(CinemachineVirtualCamera CCam in cinemachineCam)
        {
            CCam.Follow = GameObject.FindGameObjectWithTag("CinemachineTarget").transform;
        }
        
    }

}
