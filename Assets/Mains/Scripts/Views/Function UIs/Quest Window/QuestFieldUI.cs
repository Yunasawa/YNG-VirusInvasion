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

        (int Current, int Target) progress = Game.Data.RuntimeQuestStats.Quests[name].GetSerializeProgress();

        _questText.text = _node.Message.Replace("@", progress.Current.ToString());
    }

    public void UpdateQuest(string name, string value)
    {
        if (name != _questName) return;

        MDebug.Log("HMMM");

        _questText.text = _node.Message.Replace("@", value);
    }
}