using UnityEngine;

public class QuestConstruct : MonoBehaviour
{
    public string QuestName;
}
public static partial class Quest
{
    public static BaseQuest GetQuest(string name)
    {
        switch (name)
        {
            case "UpgradeFarm": return new UpgradeFarmQuest();
            default: return null;
        }
    }
}