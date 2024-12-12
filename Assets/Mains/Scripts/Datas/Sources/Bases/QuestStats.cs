using System.Collections.Generic;
using YNL.Bases;
using YNL.Utilities.Addons;

[System.Serializable]
public class QuestStats
{
    public SerializableDictionary<int, QuestStatsNode> Quests = new();
}

[System.Serializable]
public class QuestStatsNode
{
    public string Message;
    public ResourcesInfo Resource = new();
}

[System.Serializable]
public class QuestRuntime
{
    public SerializableDictionary<int, bool> CompletedQuests = new();
}