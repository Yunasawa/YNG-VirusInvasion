using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;

public class QuestBillboardUI : MonoBehaviour
{
    private QuestConstruct _questConstruct;
    private BaseQuest _quest => Game.Data.RuntimeQuestStats.Quests[_questConstruct.QuestName];

    [SerializeField] private Button _questButton;

    [SerializeField] private TextMeshProUGUI _questTitle;
    [SerializeField] private TextMeshProUGUI _questAccept;
    [SerializeField] private GameObject _progressArea;
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private Image _progressFiller;

    [SerializeField] private GameObject _exclamationMark;
    [SerializeField] private GameObject _checkMark;

    private void Awake()
    {
        _questConstruct = GetComponentInParent<QuestConstruct>();

        _questButton.onClick.AddListener(OpenQuestWindow);

        Quest.OnUpdateQuestStatus += OnUpdateQuestStatus;
    }

    private void OnDestroy()
    {
        Quest.OnUpdateQuestStatus += OnUpdateQuestStatus;
    }

    private void Start()
    {
        _questTitle.text = Game.Data.QuestStats.Quests[_questConstruct.QuestName].Title.ToUpper();

        if (Game.Data.RuntimeQuestStats.Quests.ContainsKey(_questConstruct.QuestName))
        {
            AcceptQuest();
        }
    }

    private void OpenQuestWindow()
    {
        Player.OnOpenQuestWindow?.Invoke(_questConstruct.QuestName, this);
    }
    
    public void AcceptQuest()
    {
        _questAccept.gameObject.SetActive(_quest.IsCompleted);
        _questAccept.text = _quest.IsCompleted ? "CLAIM" : "ACCEPT";

        _progressArea.SetActive(!_quest.IsCompleted);
        _progressText.gameObject.SetActive(!_quest.IsCompleted);
        _progressText.text = Game.Data.RuntimeQuestStats.Quests[_questConstruct.QuestName].GetProgress();

        _progressFiller.fillAmount = Game.Data.RuntimeQuestStats.Quests[_questConstruct.QuestName].Ratio;
    }

    public void ClaimReward()
    {
        List<ResourcesInfo> infos = Game.Data.QuestStats.Quests[_questConstruct.QuestName].Rewards;
        foreach (var info in infos) Game.Data.PlayerStats.AdjustResources(info.Type, (int)info.Amount);
        Player.OnChangeResources?.Invoke();

        Game.Data.RuntimeQuestStats.Quests.Remove(_questConstruct.QuestName);

        _questConstruct.gameObject.SetActive(false);
    }

    public void OnUpdateQuestStatus(string name, string value = "")
    {
        if (name != _questConstruct.QuestName) return;
        if (!Game.Data.RuntimeQuestStats.Quests.ContainsKey(name)) return;

        _progressText.text = _quest.GetProgress();

        if (_quest.IsCompleted)
        {
            _exclamationMark.gameObject.SetActive(false);
            _checkMark.gameObject.SetActive(true);
        }

        _progressFiller.fillAmount = Game.Data.RuntimeQuestStats.Quests[_questConstruct.QuestName].Ratio;

        _progressArea.SetActive(!_quest.IsCompleted);
        _progressText.gameObject.SetActive(!_quest.IsCompleted);

        _questAccept.gameObject.SetActive(_quest.IsCompleted);
        _questAccept.text = _quest.IsCompleted ? "CLAIM" : "ACCEPT";
    }
}