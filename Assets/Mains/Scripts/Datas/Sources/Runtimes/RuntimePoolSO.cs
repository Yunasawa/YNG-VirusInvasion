using Sirenix.OdinInspector;
using UnityEngine;
using YNL.Bases;
using YNL.Utilities.Addons;

[CreateAssetMenu(fileName = "Pool - Runtime SO", menuName = "Virus Invasion/💫 Sources/🚧 Runtime/🎬 Pool SO", order = 1)]
public class RuntimePoolSO : ScriptableObject
{
    public SerializableDictionary<StageType, GameObject> EnemyPools = new();
}