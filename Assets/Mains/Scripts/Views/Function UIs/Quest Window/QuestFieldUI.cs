using TMPro;
using UnityEngine;
using YNL.Extensions.Methods;

public class QuestFieldUI : MonoBehaviour
{
    [SerializeField] private string _questName;

    [SerializeField] private TextMeshProUGUI _questText;

    private QuestStatsNode _node;

    private void Awake()
    {
        Quest.OnUpdateQuestStatus += UpdateQuest;
    }

    private void OnDestroy()
    {
        Quest.OnUpdateQuestStatus -= UpdateQuest;
    }

    public void Initialize(string name)
    {
        _questName = name;
        _node = Game.Data.QuestStats.Quests[name];

        UpdateQuest(name, "");
    }

    public void UpdateQuest(string name, string value)
    {
        if (name != _questName) return;

        float current = Game.Data.RuntimeQuestStats.Quests[name].Current;

        string questText = _node.Message.Replace("%", _node.Replacement);
        _questText.text = questText.Replace("@", current.ToString());
    }
}