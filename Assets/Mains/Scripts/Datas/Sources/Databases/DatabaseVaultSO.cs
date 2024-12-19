
using Sirenix.OdinInspector;
using UnityEngine;
using YNL.Bases;
using YNL.Utilities.Addons;

[CreateAssetMenu(fileName = "Vault - Database SO", menuName = "Virus Invasion/💫 Sources/🚧 Database/🎬 Vault SO", order = 0)]
public class DatabaseVaultSO : ScriptableObject
{
    public SerializableDictionary<ResourceType, Sprite> ResourceIcons = new();
}