using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Header("Inverse Kinematics")]
    [SerializeField] private PlayerIK InverseKinematics;

    private int _gunPointer;

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

        IKAnim();
    }

    private void IKAnim()
    {
        Transform[] allChildren = GunParent.GetComponentsInChildren<Transform>();
        InverseKinematics.LeftHandIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftHand");
        InverseKinematics.RightHandIKTarget = allChildren.FirstOrDefault(child => child.name == "RightHand");
    }
}
