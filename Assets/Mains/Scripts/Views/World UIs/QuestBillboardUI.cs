using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Extensions.Methods;

public class QuestBillboardUI : MonoBehaviour
{
    private QuestConstruct _questConstruct;
    private BaseQuest _quest => Game.Data.RuntimeQuestStats.Quests[_questConstruct.QuestName];

    [SerializeField] private Button _questButton;

    [SerializeField] private TextMeshProUGUI _questTitle;
    [SerializeField] private GameObject _questAccept;
    [SerializeField] private GameObject _progressArea;
    [SerializeField] private TextMeshProUGUI _progressText;

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
        _questAccept.SetActive(false);
        _progressArea.SetActive(true);
        _progressText.gameObject.SetActive(true);
        _progressText.text = Game.Data.RuntimeQuestStats.Quests[_questConstruct.QuestName].GetProgress();
    }

    public void ClaimReward()
    {
        List<ResourcesInfo> infos = Game.Data.QuestStats.Quests[_questConstruct.QuestName].Resource;
        foreach (var info in infos) Game.Data.PlayerStats.AdjustResources(info.Type, (int)info.Amount);
        Player.OnChangeResources?.Invoke();

        Game.Data.RuntimeQuestStats.Quests.Remove(_questConstruct.QuestName);

        _questConstruct.gameObject.SetActive(false);
    }

    public void OnUpdateQuestStatus(string name, string value)
    {
        if (name != _questConstruct.QuestName) return;

        _progressText.text = _quest.GetProgress();

        if (_quest.IsCompleted)
        {
            _exclamationMark.gameObject.SetActive(false);
            _checkMark.gameObject.SetActive(true);
        }
    }
}