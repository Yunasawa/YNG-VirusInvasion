using System;
using YNL.Utilities.Addons;

[System.Serializable]
public class RuntimeEnemyStats
{
    public SerializableDictionary<string, EnemyRespawnTimeSpan> SMVirusRespawnTimes = new();
    public SerializableDictionary<string, int> DefeatedEnemies = new();

    public RuntimeEnemyStats()
    {
        SMVirusRespawnTimes = new() { { "SM 2 - 1", new() }, { "SM 3 - 2", new() } };
    }
}

[System.Serializable]
public class EnemyRespawnTimeSpan
{
    public int Time = 0;
    public DateTime DateTime;
}