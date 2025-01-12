using UnityEngine;

public class QuestConstruct : MonoBehaviour
{
    public string QuestName;
    public QuestBillboardUI QuestUI;

    private void Awake()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    private void Start()
    {
        if (Game.Data.RuntimeQuestStats.CompletedQuests.Contains(QuestName))
        {
            this.gameObject.SetActive(false);
        }
    }
}

public static partial class Quest
{
    public static BaseQuest GetQuest(string name)
    {
        switch (name)
        {
            case "MainQuest1": return new MainQuest1();
            case "MainQuest2": return new MainQuest2();
            case "MiniQuest1": return new MiniQuest1();
            case "MiniQuest2": return new MiniQuest2();
            case "MiniQuest3": return new MiniQuest3();
            case "MiniQuest4": return new MiniQuest4();
            case "MiniQuest5": return new MiniQuest5();
            case "MiniQuest6": return new MiniQuest6();
            case "MiniQuest7": return new MiniQuest7();
            case "MiniQuest8": return new MiniQuest8();
            default: return null;
        }
    }
}