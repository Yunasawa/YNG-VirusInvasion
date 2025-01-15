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
    public Dictionary<string, float> Quests = new();
    public List<string> CompletedQuests = new();

    public void SerializeData()
    {
        Quests.Clear();
        foreach (var pair in Game.Data.RuntimeQuestStats.Quests)
        {
            Quests.Add(pair.Key, pair.Value.Current);
        }

        CompletedQuests = Game.Data.RuntimeQuestStats.CompletedQuests;
    }

    public void DeserializeData()
    {
        Game.Data.RuntimeQuestStats.Quests.Clear();
        foreach (var pair in Quests)
        {
            var quest = Quest.GetQuest(pair.Key);
            quest.Initialize(pair.Value);
            Game.Data.RuntimeQuestStats.Quests.Add(pair.Key, quest);
            quest.OnAcceptQuest();
            
            Quest.OnDeserializeQuest.Invoke(pair.Key);
        }
    }
}
