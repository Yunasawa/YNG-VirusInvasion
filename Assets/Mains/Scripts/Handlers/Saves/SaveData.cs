using System;
using System.Collections.Generic;
using YNL.Bases;
using static YNL.Bases.PlayerStats;

[System.Serializable]
public class SaveData
{
    public (int CurrentLevel, int CurrentExp) Level = new();
    public Dictionary<ResourceType, float> Resources = new();
    public Dictionary<AttributeType, float> Bonuses = new();
    public Dictionary<AttributeType, uint> Levels = new();

    public Dictionary<string, ExtraStats> ExtraStatsLevel = new();
    public Dictionary<string, Dictionary<string, ExtraStats>> FarmStats = new();
}