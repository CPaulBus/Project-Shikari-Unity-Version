using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerIK : MonoBehaviour
{
    public Action OnIKAimAssignEvent;
    public Action OnIKIdleAssignEvent;

    [SerializeField] private PlayerGunSelector gunSelector;
    [SerializeField] private List<TwoBoneIKConstraint> _playerIK;

    private void Awake()
    {
        gunSelector = GetComponentInParent<PlayerGunSelector>();
    }

    private void Start()
    {
    }

    private void OnEnable()
    {
        OnIKAimAssignEvent += AutoAssign;
        OnIKIdleAssignEvent += IdleAssign;
    }

    private void OnDisable()
    {
        OnIKAimAssignEvent -= AutoAssign;
        OnIKIdleAssignEvent -= IdleAssign;
    }

    void AutoAssign()
    {
        if (!ThirdPersonShooterController.instance.debugMode)
        {
            switch (gunSelector.ActiveGun.Type)
            {
                case GunType.Pistol:
                    _playerIK[2].data.target.localPosition = gunSelector.ActiveGun._targetAimPosition;
                    _playerIK[2].data.target.localRotation = Quaternion.Euler(gunSelector.ActiveGun._targetAimRotation);

                    _playerIK[2].data.hint.localPosition = gunSelector.ActiveGun._hintAimPosition;
                    _playerIK[2].data.hint.localRotation = Quaternion.Euler(gunSelector.ActiveGun._hintAimRotation);

                    break;

                case GunType.Rifle:
                    _playerIK[0].data.target.localPosition = gunSelector.ActiveGun._targetAimPosition;
                    _playerIK[0].data.target.localRotation = Quaternion.Euler(gunSelector.ActiveGun._targetAimRotation);

                    _playerIK[0].data.hint.localPosition = gunSelector.ActiveGun._hintAimPosition;
                    _playerIK[0].data.hint.localRotation = Quaternion.Euler(gunSelector.ActiveGun._hintAimRotation);

                    break;

                case GunType.SciFi:
                    goto case GunType.Rifle;
            }
        }       
               
    }

    void IdleAssign()
    {
        if (!ThirdPersonShooterController.instance.debugMode)
        {
            switch (gunSelector.ActiveGun.Type)
            {
                case GunType.Pistol:
                    _playerIK[3].data.target.localPosition = gunSelector.ActiveGun._targetIdlePosition;
                    _playerIK[3].data.target.localRotation = Quaternion.Euler(gunSelector.ActiveGun._targetIdleRotation);

                    _playerIK[3].data.hint.localPosition = gunSelector.ActiveGun._hintIdlePosition;
                    _playerIK[3].data.hint.localRotation = Quaternion.Euler(gunSelector.ActiveGun._hintIdleRotation);

                    break;

                case GunType.Rifle:
                    _playerIK[1].data.target.localPosition = gunSelector.ActiveGun._targetIdlePosition;
                    _playerIK[1].data.target.localRotation = Quaternion.Euler(gunSelector.ActiveGun._targetIdleRotation);

                    _playerIK[1].data.hint.localPosition = gunSelector.ActiveGun._hintIdlePosition;
                    _playerIK[1].data.hint.localRotation = Quaternion.Euler(gunSelector.ActiveGun._hintIdleRotation);

                    break;

                case GunType.SciFi:
                    goto case GunType.Rifle;
            }
        }
            
    }
}
