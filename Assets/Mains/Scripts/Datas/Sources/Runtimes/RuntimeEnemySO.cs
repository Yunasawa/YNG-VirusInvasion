using UnityEngine;
using YNL.Utilities.Addons;

[CreateAssetMenu(fileName = "Enemy - Runtime SO", menuName = "Virus Invasion/💫 Sources/🚧 Runtime/🎬 Enemy SO", order = 0)]
public class RuntimeEnemySO : ScriptableObject
{
    public RuntimeEnemyStats EnemyStats = new();
}