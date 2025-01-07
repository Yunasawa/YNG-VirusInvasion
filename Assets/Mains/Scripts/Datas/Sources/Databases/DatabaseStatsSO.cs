using Sirenix.OdinInspector;
using UnityEngine;
using YNL.Bases;

[CreateAssetMenu(fileName = "Stats - Database SO", menuName = "Virus Invasion/💫 Sources/🚧 Database/🎬 Stats SO", order = 0)]
public class DatabaseStatsSO : ScriptableObject
{
    public ConstructStats ConstructStats = new();
    public QuestStats QuestStats = new();
    public AttributeStats AttributeStats = new();
}