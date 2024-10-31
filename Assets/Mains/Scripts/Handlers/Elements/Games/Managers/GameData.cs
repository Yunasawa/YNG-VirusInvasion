using Sirenix.OdinInspector;
using UnityEngine;
using YNL.Bases;

public class GameData : MonoBehaviour
{
    [Title("Runtime Data SO")]
    [SerializeField] private RuntimeStatsSO _runtimeStats;
    public Stats Stats => _runtimeStats.Stats;
}