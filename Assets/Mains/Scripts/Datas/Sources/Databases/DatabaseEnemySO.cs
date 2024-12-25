using UnityEngine;
using YNL.Bases;
using YNL.Utilities.Addons;

[CreateAssetMenu(fileName = "Enemy - Database SO", menuName = "Virus Invasion/💫 Sources/🚧 Database/🎬 Enemy SO", order = 1)]
public class DatabaseEnemySO : ScriptableObject
{
    public string CurrentStage = "6";
    public int CurrentEnemy = 23;
    public SerializableDictionary<string, EnemySources> EnemySources = new();

#if false
    public DefaultAsset _folder;

    [Button]
    public void GetAllEnemy()
    {
        EnemySources.Clear();

        string path = AssetDatabase.GetAssetPath(_folder);

        string[] guids = AssetDatabase.FindAssets("t:prefab", new[] { path });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            EnemySources.Add(prefab.name.RemoveWord("Monster "), new EnemySources(prefab.GetComponent<Enemy>()));
        }
    }
#endif
}

public enum StageType : byte { StageTutorial, Stage1, Stage2, Stage3, Stage4, Stage5, Stage6, Stage7 }