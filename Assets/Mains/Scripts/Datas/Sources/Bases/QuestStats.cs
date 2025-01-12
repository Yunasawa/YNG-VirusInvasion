using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
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
    public string Title;
    public string Message;
    public string Target;
    public string Replacement;
    public List<ResourcesInfo> Rewards = new();
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
            SerializeQuestNode node = new(pair.Value.IsCompleted, pair.Value.Current);
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
            quest.Initialize(pair.Value.IsCompleted, pair.Value.Current);
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
    public float Current;

    public SerializeQuestNode(bool isCompleted, float current)
    {
        IsCompleted = isCompleted;
        Current = current;
    }
}
