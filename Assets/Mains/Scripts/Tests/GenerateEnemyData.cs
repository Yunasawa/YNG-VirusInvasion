using Sirenix.OdinInspector;
using UnityEngine;

public class GenerateEnemyData : MonoBehaviour
{
    [SerializeField] private DatabaseEnemySO _database;

    [Button]
    public void GenerateText()
    {
        string text = "";

        foreach (var enemy in _database.EnemySources)
        {
            text += $"{enemy.Key}, {enemy.Value.RespawnTime}, {enemy.Value.Exp}, {enemy.Value.HP}, ";
            foreach (var drop in enemy.Value.Drops)
            {
                text += $"({drop.Key}, {drop.Value})";
            }
            text += "\n";
        }

        GUIUtility.systemCopyBuffer = text;
    }
}