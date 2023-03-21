using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

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
    private bool _allowBulletSpawn = true;

    [Header("==Debug Mode==")]
    [SerializeField] private bool _debugMode;
    [SerializeField] private Transform debugTransform;
    private bool allowInvoke = true;

    [Header("==Rigs==")]
    [SerializeField] private Rig[] rifleRigs;
    private float aimRigWeight;

    [Header("Weapon Equipped")]
    [SerializeField] private bool _isRifleEquipped;
    [SerializeField] private bool _isPistolEquipped;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private Animator _animator;

    [SerializeField] private int playerID;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (_debugMode)
        {
            debugTransform.gameObject.SetActive(true);
        }
        else
            debugTransform.gameObject.SetActive(false);

        spawnBulletPosition = GameObject.FindGameObjectWithTag("BulletSpawn").GetComponent<Transform>();

        if (_isRifleEquipped)
        {
            _animator.SetLayerWeight(0, 0f);
        }
        else
        {
            _animator.SetLayerWeight(1, 0f);
        }
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = MouseWorldFunction();
        AimFunction(mouseWorldPosition);


        if (_isRifleEquipped)
        {
            rifleRigs[0].weight = Mathf.Lerp(rifleRigs[0].weight, aimRigWeight, Time.deltaTime * 20f);
        }

        if (_isPistolEquipped)
        {
            foreach(Rig currRig in rifleRigs)
            {
                currRig.weight = 0f;
            }
        }
    }

    private void AimFunction(Vector3 mouseWorldPosition)
    {
        if (starterAssetsInputs.aim)
        {
            OnAimStarted();

            if(_isRifleEquipped)
                _animator.SetLayerWeight(2, Mathf.Lerp(_animator.GetLayerWeight(2), 1f, Time.deltaTime * 10f));
            
            if(_isPistolEquipped)
                _animator.SetLayerWeight(3, Mathf.Lerp(_animator.GetLayerWeight(3), 1f, Time.deltaTime * 10f));

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

            ShootFunction(mouseWorldPosition);
        }
        else
        {
            OnAimFinished();

            if (_isRifleEquipped)
                _animator.SetLayerWeight(2, Mathf.Lerp(_animator.GetLayerWeight(2), 0f, Time.deltaTime * 10f));

            if (_isPistolEquipped)
                _animator.SetLayerWeight(3, Mathf.Lerp(_animator.GetLayerWeight(3), 0f, Time.deltaTime * 10f));
        }
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

    private void ShootFunction(Vector3 mouseWorldPosition)
    {
        if (starterAssetsInputs.shoot && _allowBulletSpawn)
        {
            Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
            Instantiate(pfBulletParticle, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            _allowBulletSpawn = false;
            _animator.SetTrigger("Fire");

            if (allowInvoke)
            {
                Invoke("BulletReset", _bulletDelay);
                allowInvoke = false;
            }
        }
    }

    private void OnAimFinished()
    {
        aimVirtualCamera.gameObject.SetActive(false);
        thirdPersonController.SetSensitivity(normalSensitivity);
        thirdPersonController.SetRotateOnMove(true);

        if(_isRifleEquipped)
            aimRigWeight = 0f;
    }

    private void OnAimStarted()
    {
        aimVirtualCamera.gameObject.SetActive(true);
        thirdPersonController.SetSensitivity(aimSensitivity);
        thirdPersonController.SetRotateOnMove(false);

        if (_isRifleEquipped)
            aimRigWeight = 1f;
    }

    void BulletReset()
    {
        _allowBulletSpawn = true;
        allowInvoke = true;
    }
}
