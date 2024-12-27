using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Extensions.Methods;

public class QuestFieldUI : MonoBehaviour
{
    [SerializeField] private string _questName;

    [SerializeField] private TextMeshProUGUI _questText;
    [SerializeField] private TextMeshProUGUI _rewardText;

    private QuestBillboardUI _ui;

    private void Awake()
    {
        Player.OnSendQuestUI += OnSendQuestUI;
    }

    private void OnDestroy()
    {
        Player.OnSendQuestUI -= OnSendQuestUI;
    }

    public void Initialize(params object[] values)
    {
        _questText.text = Game.Data.QuestStats.Quests[_questName].Message.Replace("@", values[0].ToString());
        _rewardText.text = $"{Game.Data.QuestStats.Quests[_questName].Resource.Type}\n{Game.Data.QuestStats.Quests[_questName].Resource.Amount}";

        if (!Game.Data.QuestRuntime.CurrentQuests.ContainsKey(_questName)) this.gameObject.SetActive(false);
        else this.gameObject.SetActive(true);
    }

    private void OnSendQuestUI(string name, QuestBillboardUI ui)
    {
        if (name != _questName) return;
        _ui = ui;
    }

    public void UpdateQuest(params object[] values)
    {
        if (!Game.Data.QuestRuntime.CurrentQuests.ContainsKey(_questName)) return;

        _questText.text = Game.Data.QuestStats.Quests[_questName].Message.Replace("@", values[0].ToString());
        if (!_ui.IsNull() && !values.IsNullOrEmpty()) _ui.SetProgressText($"{values[0]}/2");
    }

    public void ShowReward()
    {
        Game.Data.QuestRuntime.CurrentQuests[_questName] = true;
        Player.OnFinishQuest?.Invoke(_questName);
    }

    private void GetReward()
    {
        Game.Data.PlayerStats.AdjustResources(Game.Data.QuestStats.Quests[_questName].Resource.Type, (int)Game.Data.QuestStats.Quests[_questName].Resource.Amount);
        this.gameObject.SetActive(false);
    }
}