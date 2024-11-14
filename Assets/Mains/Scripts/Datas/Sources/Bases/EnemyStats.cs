using Sirenix.OdinInspector;
using YNL.Bases;
using YNL.Utilities.Addons;

namespace YNL.Bases
{
    [System.Serializable]
    public class EnemyStats
    {
        public SerializableDictionary<uint, MonsterStatsNode> Stats = new();

        [Button]
        public void AddNewStats() => Stats.Add((uint)Stats.Count, new());
    }
}

[System.Serializable]
public class MonsterStatsNode
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