using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuntimeMiniCell
{
    public List<DeliveryCellStats> DeliveryCells = new();

    public RuntimeMiniCell()
    {
        for (int i = 0; i < 10; i++)
        {
            DeliveryCells.Add(new());
        }
    }
}

[System.Serializable]
public class MiniCellStats
{
    public SerializableVector3 Position;
}

[System.Serializable]
public class DeliveryCellStats : MiniCellStats
{
    public bool TargetIsHome = false;
    public List<Group<string, int>> DeliveriedEnemies = new();
}

[System.Serializable]
public class HunterCellStats : MiniCellStats
{
    
}