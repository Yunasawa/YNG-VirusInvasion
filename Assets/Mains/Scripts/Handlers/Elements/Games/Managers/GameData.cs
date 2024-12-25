using Sirenix.OdinInspector;
using UnityEngine;
using YNL.Bases;
using YNL.Utilities.Addons;

public class GameData : MonoBehaviour
{
    [SerializeField] private RuntimeStatsSO _runtimeStats;
    [SerializeField] private DatabaseStatsSO _databaseStats;
    [SerializeField] private DatabaseEnemySO _databaseEnemy;

    public DatabaseVaultSO Vault;

    public SerializableDictionary<StageType, Boundary> BoundaryStages = new();

    public PlayerStats PlayerStats => _runtimeStats.PlayerStats;
    public CapacityStats CapacityStats => _runtimeStats.CapacityStats;
    public QuestRuntime QuestRuntime => _runtimeStats.QuestRuntime;

    public ConstructStats ConstructStats => _databaseStats.ConstructStats;
    public QuestStats QuestStats => _databaseStats.QuestStats;

    public SerializableDictionary<string, EnemySources> EnemySources => _databaseEnemy.EnemySources;

    public void MonoAwake()
    {
        _runtimeStats.Reset();
    }
}