using Sirenix.OdinInspector;
using UnityEngine;
using YNL.Bases;

[CreateAssetMenu(fileName = "Stats - Runtime SO", menuName = "Virus Invasion/💫 Sources/🚧 Runtime/🎬 Stats SO", order = 0)]
public class RuntimeStatsSO : ScriptableObject
{
    public PlayerStats PlayerStats = new();
    public CapacityStats CapacityStats = new();
    public QuestRuntime QuestRuntime = new();

    public void Reset()
    {
        PlayerStats.Reset();
        CapacityStats.Reset();
    }
}