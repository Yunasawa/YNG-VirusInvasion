using Sirenix.OdinInspector;
using UnityEngine;
using YNL.Bases;
using YNL.Utilities.Addons;

public class GameData : MonoBehaviour
{
    [Title("Runtime Data SO")]
    [SerializeField] private RuntimeStatsSO _runtimeStats;
    public PlayerStats Stats => _runtimeStats.Stats;

    [Title("Enemey Stats")]
    public SerializableDictionary<uint, MonsterStats> MonsterStats = new();
}