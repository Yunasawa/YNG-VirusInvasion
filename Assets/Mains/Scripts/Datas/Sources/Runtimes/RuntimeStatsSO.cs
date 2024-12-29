using Sirenix.OdinInspector;
using UnityEngine;
using YNL.Bases;
using YNL.Utilities.Addons;

[CreateAssetMenu(fileName = "Stats - Runtime SO", menuName = "Virus Invasion/💫 Sources/🚧 Runtime/🎬 Stats SO", order = 0)]
public class RuntimeStatsSO : ScriptableObject
{
    public PlayerStats PlayerStats = new();
    public CapacityStats CapacityStats = new();
    public RuntimeConstructStats ConstructStats = new();
    public QuestRuntime QuestRuntime = new();

    public void Reset()
    {
        PlayerStats.Reset();
        CapacityStats.Reset();
        ConstructStats.Reset();
    }
}