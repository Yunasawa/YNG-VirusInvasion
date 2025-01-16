using Sirenix.OdinInspector;
using UnityEngine;
using YNL.Bases;
using YNL.Utilities.Addons;

[CreateAssetMenu(fileName = "Enemy - Database SO", menuName = "Virus Invasion/💫 Sources/🚧 Database/🎬 Enemy SO", order = 1)]
public class DatabaseEnemySO : ScriptableObject
{
    public string CurrentStage = "6";
    public int CurrentEnemy = 23;
    public SerializableDictionary<string, EnemySources> EnemySources = new();

    [Button]
    public void Assign()
    {
        foreach (var enemy in EnemySources)
        {
            int stage = int.Parse(enemy.Key.Substring(6, 1));
            enemy.Value.Capacity = stage + Random.Range(stage / 2 + 1, stage);
        }
    }
}

public enum StageType : byte { StageTutorial, Stage1, Stage2, Stage3, Stage4, Stage5, Stage6, Stage7 }