using UnityEngine;
using YNL.Bases;

[CreateAssetMenu(fileName = "Stats - Runtime SO", menuName = "Virus Invasion/💫 Sources/🚧 Runtime/🎬 Stats SO", order = 0)]
public class RuntimeStatsSO : ScriptableObject
{
    public PlayerStats PlayerStats = new();
    public CapacityStats CapacityStats = new();
    public RuntimeConstructStats ConstructStats = new();
    public RuntimeQuestStats RuntimeQuestStats = new();
    public RuntimeMiniCell RuntimeMiniCell = new();

    public void Reset()
    {
        PlayerStats.Reset();
        CapacityStats.Reset();
        ConstructStats.Reset();
    }
}