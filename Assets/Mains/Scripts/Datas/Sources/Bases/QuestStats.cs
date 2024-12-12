using System.Collections.Generic;
using YNL.Bases;
using YNL.Utilities.Addons;

[System.Serializable]
public class QuestStats
{
    public SerializableDictionary<string, QuestStatsNode> Quests = new();
}

[System.Serializable]
public class QuestStatsNode
{
    public string RawMessage;
    public string Message;
    public string Title;
    public ResourcesInfo Resource = new();
}

[System.Serializable]
public class QuestRuntime
{
    public SerializableDictionary<string, bool> CurrentQuests = new();
}