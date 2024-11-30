using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Addons;
using YNL.Utilities.Addons;

[CreateAssetMenu(fileName = "Enemy - Database SO", menuName = "Virus Invasion/💫 Sources/🚧 Database/🎬 Enemy SO", order = 1)]
public class DatabaseEnemySO : ScriptableObject
{
    public SerializableDictionary<uint, EnemySources> EnemySources = new();
}