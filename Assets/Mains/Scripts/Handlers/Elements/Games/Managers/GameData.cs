using Sirenix.OdinInspector;
using UnityEngine;
using YNL.Bases;
using YNL.Utilities.Addons;

public class GameData : MonoBehaviour
{
    [SerializeField] private RuntimeStatsSO _runtimeStats;
    [SerializeField] private DatabaseStatsSO _databaseStats;
    [SerializeField] private DatabaseEnemySO _databaseEnemy;

    public SerializableDictionary<StageType, Boundary> BoundaryStages = new();

    public PlayerStats PlayerStats => _runtimeStats.PlayerStats;
    public CapacityStats CapacityStats => _runtimeStats.CapacityStats;
    public ConstructStats ConstructStats => _databaseStats.ConstructStats;

    public SerializableDictionary<string, EnemySources> EnemySources => _databaseEnemy.EnemySources;
    public SerializableDictionary<StageType, string> EnemyStages => _databaseEnemy.EnemyStages;
}