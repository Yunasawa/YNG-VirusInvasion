using UnityEngine;

public class QuestConstruct : MonoBehaviour
{
    public string QuestName;
    public GameObject QuestUI;
}

public static partial class Quest
{
    public static BaseQuest GetQuest(string name)
    {
        switch (name)
        {
            case "MiniQuest1": return new MiniQuest1();
            case "MiniQuest2": return new MiniQuest2();
            case "MainQuest1": return new MainQuest1();
            default: return null;
        }
    }
}