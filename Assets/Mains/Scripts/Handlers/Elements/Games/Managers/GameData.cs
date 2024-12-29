using Sirenix.OdinInspector;
using UnityEngine;
using YNL.Bases;
using YNL.Utilities.Addons;

public class GameData : MonoBehaviour
{
    public RuntimeStatsSO RuntimeStats;
    [SerializeField] private DatabaseStatsSO _databaseStats;
    [SerializeField] private DatabaseEnemySO _databaseEnemy;

    public DatabaseVaultSO Vault;

    public SerializableDictionary<StageType, Boundary> BoundaryStages = new();

    public PlayerStats PlayerStats { get => RuntimeStats.PlayerStats; set => RuntimeStats.PlayerStats = value; }
    public CapacityStats CapacityStats {get => RuntimeStats.CapacityStats; set => RuntimeStats.CapacityStats = value; }
    public QuestRuntime QuestRuntime => RuntimeStats.QuestRuntime;

    public ConstructStats ConstructStats => _databaseStats.ConstructStats;
    public QuestStats QuestStats => _databaseStats.QuestStats;

    public SerializableDictionary<string, EnemySources> EnemySources => _databaseEnemy.EnemySources;

    public void MonoAwake()
    {
        RuntimeStats.Reset();
    }
}