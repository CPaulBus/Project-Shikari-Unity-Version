using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Create Player", order = 0)]
public class PlayerCharacterSO : ScriptableObject
{
    public CharSelect characterType;

    [Header("Attributes")]
    public float Health;
    public float MaxHealth;
    public float Armor;
    public float MaxArmor;

    //public int Level;
    //public float experience;
    //public float maxExperience;

    public GameObject ModelPrefab;
    private GameObject Model;

    private MonoBehaviour ActiveMonoBehaviour;

    public void Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour)
    {
        this.ActiveMonoBehaviour = ActiveMonoBehaviour;

        Model = Instantiate(ModelPrefab);
    }
}