using System.Collections.Generic;
using UnityEngine;
using YNL.Bases;
using YNL.Utilities.Addons;

[System.Serializable]
public class RuntimeMiniCell
{
    public List<DeliveryCellStats> DeliveryCells = new();
    public List<HunterCellStats> HunterCells = new();

    public RuntimeMiniCell()
    {
        for (int i = 0; i < 10; i++)
        {
            DeliveryCells.Add(new());
            HunterCells.Add(new());
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
    public int Capacity;
    public SerializableDictionary<ResourceType, int> Resources = new();
}