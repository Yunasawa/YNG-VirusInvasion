using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestFieldUI : MonoBehaviour
{
    [SerializeField] private int _stage;

    [SerializeField] private TextMeshProUGUI _questText;
    [SerializeField] private Button _questReward;
    [SerializeField] private TextMeshProUGUI _rewardText;

    private bool _isFinished = false;
    private bool _isCompleted = false;

    private void Awake()
    {
        _questReward.onClick.AddListener(GetReward);
    }

    public void Initialize(params object[] values)
    {
        _questText.text = Game.Data.QuestStats.Quests[_stage].Message.Replace("@", values[0].ToString());
        _rewardText.text = $"{Game.Data.QuestStats.Quests[_stage].Resource.Type}\n{Game.Data.QuestStats.Quests[_stage].Resource.Amount}";

        if (!_isFinished) _questReward.interactable = false;

        if (Game.Data.PlayerStats.CurrentLevel < _stage || _isCompleted || Game.Data.QuestRuntime.CompletedQuests[_stage]) this.gameObject.SetActive(false);
        else this.gameObject.SetActive(true);
    }

    public void UpdateQuest(params object[] values)
    {
        _questText.text = Game.Data.QuestStats.Quests[_stage].Message.Replace("@", values[0].ToString());
    }

    public void ShowReward()
    {
        _questReward.interactable = true;
        _isFinished = true;
    }

    private void GetReward()
    {
        Game.Data.PlayerStats.AdjustResources(Game.Data.QuestStats.Quests[_stage].Resource.Type, (int)Game.Data.QuestStats.Quests[_stage].Resource.Amount);
        _isCompleted = Game.Data.QuestRuntime.CompletedQuests[_stage] = true;
        this.gameObject.SetActive(false);
    }
}