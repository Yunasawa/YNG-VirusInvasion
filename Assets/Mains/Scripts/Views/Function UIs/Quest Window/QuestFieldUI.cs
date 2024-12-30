using TMPro;
using UnityEngine;

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

        _questText.text = _node.Message.Replace("@", "0");
    }

    public void UpdateQuest(string name, string value)
    {
        if (name != _questName) return;

        _questText.text = _node.Message.Replace("@", value);
    }
}