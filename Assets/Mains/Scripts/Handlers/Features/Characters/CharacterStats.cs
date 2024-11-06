using UnityEngine;
using YNL.Bases;
using YNL.Utilities.Addons;

public class CharacterStats : MonoBehaviour
{
    [HideInInspector] public CharacterManager Manager;

    public MonsterStats Stats;

    public (uint Current, uint Max) Health;

    private void Awake()
    {
        Manager = GetComponent<CharacterManager>();
    }

    private void Start()
    {
        Health.Max = Stats.HP;
    }

    public void StartDamage(float dps)
    {

    }
}

[System.Serializable]
public struct MonsterStats
{
    public float Exp;
    public uint Capacity;
    public uint HP;
    public uint MS;
    public uint MaxInstancePerArea;
    public float SpawningTime;
    public SerializableDictionary<ResourceType, uint> Drops;
    public float AttackDamage;
    public float ExperimentMultiplier;
}