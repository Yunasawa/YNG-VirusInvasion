using System;
using System.Collections.Generic;
using UnityEngine;
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

    public SerializableVector3 CurrentPosition;

    public uint CurrentCapacity;
    public Dictionary<ResourceType, uint> CapacityResources = new();

    public StageType CurrentStage;
}

[Serializable]
public struct SerializableVector3
{
    public float X, Y, Z;

    public SerializableVector3(Vector3 vector)
    {
        X = vector.x;
        Y = vector.y;
        Z = vector.z;
    }

    public Vector3 ToVector3() => new Vector3(X, Y, Z);

    public override string ToString() => $"({X}, {Y}, {Z})";
}