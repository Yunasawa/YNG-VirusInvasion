using System.Collections.Generic;
using YNL.Bases;
using YNL.Extensions.Methods;
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
    public List<ResourcesInfo> Resource = new();
}

[System.Serializable]
public class RuntimeQuestStats
{
    public SerializableDictionary<string, BaseQuest> Quests = new();
    public List<string> CompletedQuests = new();
}

[System.Serializable]
public class SerializeQuestStats
{
    public Dictionary<string, SerializeQuestNode> Quests = new();
    public List<string> CompletedQuests = new();

    public void SerializeData()
    {
        Quests.Clear();
        foreach (var pair in Game.Data.RuntimeQuestStats.Quests)
        {
            SerializeQuestNode node = new(pair.Value.IsCompleted, pair.Value.GetSerializeProgress());
            Quests.Add(pair.Key, node);
        }

        CompletedQuests = Game.Data.RuntimeQuestStats.CompletedQuests;
    }

    public void DeserializeData()
    {
        Game.Data.RuntimeQuestStats.Quests.Clear();
        foreach (var pair in Quests)
        {
            var quest = Quest.GetQuest(pair.Key);
            quest.Initialize(pair.Value.IsCompleted, pair.Value.Target, pair.Value.Current);
            Game.Data.RuntimeQuestStats.Quests.Add(pair.Key, quest);
            quest.OnAcceptQuest();
            
            Quest.OnDeserializeQuest.Invoke(pair.Key);
        }
    }
}

[System.Serializable]
public class SerializeQuestNode
{
    public bool IsCompleted;
    public int Target;
    public int Current;

    public SerializeQuestNode(bool isCompleted, (int, int) progress)
    {
        IsCompleted = isCompleted;
        Current = progress.Item1;
        Target = progress.Item2;
    }
}