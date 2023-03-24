using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunSelector : MonoBehaviour
{
    [SerializeField]
    private GunName Gun;
    [SerializeField]
    private Transform GunParent;
    [SerializeField]
    private List<GunScriptableObject> Guns;

    [Space]
    [Header("Runtime Filled")]
    public GunScriptableObject ActiveGun;

    private ThirdPersonController _thirdPersonControl;
    private StarterAssetsInputs _starterAssetsInputs;

    private int _gunPointer;

    private void Awake()
    {
        _thirdPersonControl = GetComponent<ThirdPersonController>();
        _starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    private void Start()
    {
        GunScriptableObject gun = Guns.Find(gun => gun.Name == Gun);

        if (gun == null)
        {
            Debug.LogError($"No GunScriptableObject found for GunType: {gun}");
            return;
        }

        ActiveGun = gun;
        gun.Spawn(GunParent, this);
    }

    void Update()
    {
        if (_starterAssetsInputs.switchWeapon)
        {
            SwitchWeapon();
        }
    }

    void SwitchWeapon()
    {
        ActiveGun.DeSpawn();
        ActiveGun = null;

        _gunPointer = Guns.FindIndex(gun => gun.Name == Gun);
        Debug.Log($"Current Gun Pointer: {_gunPointer}");

        if (_gunPointer < Guns.Count-1)
        {
            _gunPointer++;
            ChangeGun();
        }
        else
        {
            _gunPointer = 0;
            ChangeGun();
        }

        ThirdPersonShooterController.instance.OnSwitchWeaponEvent?.Invoke();
        _starterAssetsInputs.switchWeapon = false;
    }

    private void ChangeGun()
    {
        GunScriptableObject gun = Guns[_gunPointer];

        Gun = gun.Name;

        if (gun == null)
        {
            Debug.LogError($"No GunScriptableObject found for GunType: {gun}");
        }

        ActiveGun = gun;
        gun.Spawn(GunParent, this);
    }
}
