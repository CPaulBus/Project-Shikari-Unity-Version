using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using System;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();

    [Header("==Bullet Spawn==")]
    [SerializeField] private GameObject pfBulletParticle;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _bulletDelay;

    [Header("==Debug Mode==")]
    [SerializeField] public bool debugMode;
    [SerializeField] private Transform debugTransform;

    [Header("==Player IK==")]
    [SerializeField] private List<RigLayer> rigs;
    [SerializeField] private PlayerIK _pIK;

    [Header("Weapon Equipped")]
    [SerializeField] private PlayerGunSelector GunSelector;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private Animator _animator;

    public static ThirdPersonShooterController instance;

    private bool _isAiming;

    [SerializeField] private int playerID;

    public Action OnSwitchWeaponEvent;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        _animator = GetComponent<Animator>();
        rigs = GetComponent<RigBuilder>().layers;
        GunSelector = GetComponent<PlayerGunSelector>();
        _pIK = GetComponentInChildren<PlayerIK>();

        aimVirtualCamera = GameObject.Find("PlayerAimCamera").GetComponent<CinemachineVirtualCamera>();

        instance = this;
    }

    private void OnEnable()
    {
        OnSwitchWeaponEvent += IdleGunRig;
    }

    private void OnDisable()
    {
        OnSwitchWeaponEvent -= IdleGunRig;
    }

    private void Start()
    {
        if (debugMode)
        {
            debugTransform.gameObject.SetActive(true);
        }
        else
            debugTransform.gameObject.SetActive(false);

        foreach (RigLayer currRig in rigs)      //Checking if all rigs has greater than 0
        {
            if (currRig.rig.weight > 0)
                currRig.rig.weight = 0f;
        }
    }

    private void IdleGunRig()
    {
        if (!_isAiming)
        {
            switch (GunSelector.ActiveGun.Type)
            {
                case GunType.SciFi:
                    if (rigs[3].rig.weight != 0f) 
                    {
                        rigs[3].rig.weight = 0f;
                    }

                    rigs[1].rig.weight = 1f;
                    rigs[0].rig.weight = 0f;
                    break;

                case GunType.Rifle:
                    goto case GunType.SciFi;

                case GunType.Pistol:
                    if (rigs[1].rig.weight != 0f)
                    {
                        rigs[1].rig.weight = 0f;
                    }

                    rigs[3].rig.weight = 1f;
                    rigs[2].rig.weight = 0f;
                    break;
                
            }
        }
        else
        {
            switch (GunSelector.ActiveGun.Type)
            {
                case GunType.SciFi:
                    if (rigs[2].rig.weight != 0f)
                    {
                        rigs[2].rig.weight = 0f;
                    }

                    rigs[1].rig.weight = 0f;
                    rigs[0].rig.weight = 1f;
                    break;

                case GunType.Rifle:
                    goto case GunType.SciFi;

                case GunType.Pistol:
                    if (rigs[0].rig.weight != 0f)
                    {
                        rigs[0].rig.weight = 0f;
                    }

                    rigs[3].rig.weight = 0f;
                    rigs[2].rig.weight = 1f;
                    break;                
            }
        }

        _pIK.OnIKIdleAssignEvent?.Invoke();
        _pIK.OnIKAimAssignEvent?.Invoke();
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = MouseWorldFunction();
        AimFunction(mouseWorldPosition);
    }

    private void LateUpdate()
    {
        IdleGunRig();
        AnimCheckFunc();    // Check Current Gun Type
    }

    void AnimCheckFunc()
    {
        switch (GunSelector.ActiveGun.Type)
        {
            case GunType.SciFi:
                _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
                _animator.SetLayerWeight(3, Mathf.Lerp(_animator.GetLayerWeight(3), 0f, Time.deltaTime * 10f));
                break;

            case GunType.Pistol:
                _animator.SetLayerWeight(3, Mathf.Lerp(_animator.GetLayerWeight(3), 1f, Time.deltaTime * 10f));
                _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
                break;

            case GunType.Rifle:
                goto case GunType.SciFi;
        }
    }

    private IEnumerator delayFunc()
    {
        yield return new WaitForSeconds(0.5f);
    }

    private void AimFunction(Vector3 mouseWorldPosition)
    {
        if (starterAssetsInputs.aim)
        {
            OnAimStarted();
            AnimatorAimLayer(true);
            AimingFix(mouseWorldPosition);
            //ShootFunction(mouseWorldPosition); **Old Shoot Function
            if(GunSelector.ActiveGun != null)
            {
                GunSelector.ActiveGun.Tick(starterAssetsInputs.shoot);

                if (starterAssetsInputs.shoot)
                {
                    _animator.SetTrigger("Fire");
                }
            }
        }
        else
        {
            OnAimFinished();
            AnimatorAimLayer(false);
        }
    }

    private void AnimatorAimLayer(bool aimStart)
    {
        if (aimStart)
        {
            switch (GunSelector.ActiveGun.Type)
            {
                case GunType.SciFi:
                    _animator.SetLayerWeight(2, Mathf.Lerp(_animator.GetLayerWeight(2), 1f, Time.deltaTime * 10f));
                    _animator.SetLayerWeight(4, Mathf.Lerp(_animator.GetLayerWeight(4), 0f, Time.deltaTime * 10f));

                    rigs[0].rig.weight = Mathf.Lerp(rigs[0].rig.weight, 1f, Time.deltaTime * 20f);
                    break;
                case GunType.Pistol:
                    _animator.SetLayerWeight(4, Mathf.Lerp(_animator.GetLayerWeight(4), 1f, Time.deltaTime * 10f));
                    _animator.SetLayerWeight(2, Mathf.Lerp(_animator.GetLayerWeight(2), 0f, Time.deltaTime * 10f));

                    rigs[2].rig.weight = Mathf.Lerp(rigs[2].rig.weight, 1f, Time.deltaTime * 20f);
                    break;

                case GunType.Rifle:
                    goto case GunType.SciFi;
            }                
        }
        else
        {
            switch (GunSelector.ActiveGun.Type)
            {
                case GunType.SciFi:
                    _animator.SetLayerWeight(2, Mathf.Lerp(_animator.GetLayerWeight(2), 0f, Time.deltaTime * 10f));
                    rigs[0].rig.weight = Mathf.Lerp(rigs[0].rig.weight, 0f, Time.deltaTime * 20f);
                    break;
                case GunType.Pistol:
                    _animator.SetLayerWeight(4, Mathf.Lerp(_animator.GetLayerWeight(4), 0f, Time.deltaTime * 10f));
                    rigs[2].rig.weight = Mathf.Lerp(rigs[2].rig.weight, 0f, Time.deltaTime * 20f);
                    break;

                case GunType.Rifle:
                    goto case GunType.SciFi;
            }
        }
        
    }

    private void AimingFix(Vector3 mouseWorldPosition)
    {
        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
    }

    private Vector3 MouseWorldFunction()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }
        else
        {
            mouseWorldPosition = ray.GetPoint(10);
        }

        return mouseWorldPosition;
    }

    private void OnAimFinished()
    {
        aimVirtualCamera.gameObject.SetActive(false);
        thirdPersonController.SetSensitivity(normalSensitivity);
        thirdPersonController.SetRotateOnMove(true);

        _isAiming = false;
    }

    private void OnAimStarted()
    {
        aimVirtualCamera.gameObject.SetActive(true);
        thirdPersonController.SetSensitivity(aimSensitivity);
        thirdPersonController.SetRotateOnMove(false);

        _pIK.OnIKAimAssignEvent?.Invoke();

        _isAiming = true;
    }
}
