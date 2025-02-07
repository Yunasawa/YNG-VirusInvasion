using System;
using YNL.Utilities.Addons;

[System.Serializable]
public class RuntimeEnemyStats
{
    public SerializableDictionary<string, EnemyRespawnTimeSpan> SMVirusRespawnTimes = new();

    public RuntimeEnemyStats()
    {
        SMVirusRespawnTimes = new() { { "SM 2 - 1", new() }, { "SM 3 - 2", new() } };
    }
}

[System.Serializable]
public class EnemyRespawnTimeSpan
{
    public int Time;
    public DateTime DateTime;
}