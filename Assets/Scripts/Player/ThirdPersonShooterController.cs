using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ThirdPersonShooterController : MonoBehaviour
{
    [Header("==AIMING==")]
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity = 1f;
    [SerializeField] private float aimSensitivity = 0.8f;
    [SerializeField] private LayerMask aimColliderMask = new LayerMask();

    [Space(1f)]
    [Header("==DEBUG MODE==")]
    public bool debugMode;
    [SerializeField] private Transform debugTransform;

    [Space(1f)]
    [Header("==PLAYER GUN==")]
    [SerializeField] private PlayerGunSelector gunSelector;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private Animator animator;
    private RigBuilder rigBuilder;

    private bool isAiming;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
        rigBuilder = GetComponent<RigBuilder>();
    }

    private void Start()
    {
        rigBuilder.layers[0].rig.weight = 0;
        debugTransform.gameObject.SetActive(debugMode?true:false);
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = MouseWorldFunction();
        AimFunction(mouseWorldPosition);
    }

    private Vector3 MouseWorldFunction()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width/2f, Screen.height/2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray,out RaycastHit raycastHit,999f, aimColliderMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition=raycastHit.point;
        }
        else
        {
            mouseWorldPosition = ray.GetPoint(10);
        }

        return mouseWorldPosition;
    }

    private void AimFunction(Vector3 mouseWorldPosition)
    {
        if (starterAssetsInputs.aim)
        {
            rigBuilder.layers[0].rig.weight = 1;
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);

            AimingFix(mouseWorldPosition);

            if (gunSelector.ActiveGun != null)
            {
                gunSelector.ActiveGun.Tick(starterAssetsInputs.shoot);

                if (starterAssetsInputs.shoot)
                {
                    animator.SetTrigger("Fire");
                }
            }
        }
        else
        {
            rigBuilder.layers[0].rig.weight = 0;
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
        }

        void AimingFix(Vector3 mouseWorldPosition)
        {
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
    }
}
