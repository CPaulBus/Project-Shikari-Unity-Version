using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunSelector : MonoBehaviour
{
    [SerializeField]
    private GunType Gun;
    [SerializeField]
    private Transform GunParent;
    [SerializeField]
    private List<GunScriptableObject> Guns;
    //[SerializeField]
    //private PlayerIK InverseKinematics;

    [Space]
    [Header("Runtime Filled")]
    public GunScriptableObject ActiveGun;

    private void Start()
    {
        GunScriptableObject gun = Guns.Find(gun => gun.Type == Gun);

        if(gun == null)
        {
            Debug.LogError($"No GunScriptableObject found for GunType: {gun}");
            return;
        }

        ActiveGun = gun;
        gun.Spawn(GunParent, this);
    }
}
