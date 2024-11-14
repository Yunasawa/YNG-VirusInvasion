using Sirenix.OdinInspector;
using UnityEngine;
using YNL.Bases;

[CreateAssetMenu(fileName = "Stats - Runtime SO", menuName = "Virus Invasion/💫 Sources/🚧 Runtime/🎬 Stats SO", order = 0)]
public class RuntimeStatsSO : ScriptableObject
{
    public PlayerStats PlayerStats = new();
}