using Sirenix.OdinInspector;
using System;
using System.Security.Claims;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Utilities.Addons;

public class QuestWindowUI : MonoBehaviour
{
    [Title("Quest Field")]
    [SerializeField] private SerializableDictionary<string, QuestFieldUI> _questFields = new();
    [SerializeField] private QuestFieldUI _questFieldPrefab;

    [SerializeField] private Button _expandingButton;
    [SerializeField] private Image _expandingArrow;
    [SerializeField] private GameObject _questPanel;

    private bool _isExpanded = false;

    [Title("Quest Window")]
    [SerializeField] private GameObject _questWindow;
    [SerializeField] private Button _closeButton;
    [SerializeField] private TextMeshProUGUI _windowTitle;
    [SerializeField] private TextMeshProUGUI _questMessage;
    [SerializeField] private TextMeshProUGUI _questProgress;
    [SerializeField] private TextMeshProUGUI _questReward;
    [SerializeField] private Button _questButton;
    [SerializeField] private TextMeshProUGUI _buttonText;

    private QuestBillboardUI _ui;

    private void Awake()
    {
        Player.OnOpenQuestWindow += OnOpenQuestWindow;

        _expandingButton.onClick.AddListener(ExpandPanel);
        _closeButton.onClick.AddListener(() => _questWindow.SetActive(false));
    }

    private void OnDestroy()
    {
        Player.OnOpenQuestWindow -= OnOpenQuestWindow;
    }

    private void Start()
    {
        _questPanel.SetActive(false);
    }

    private void OnOpenQuestWindow(string name, QuestBillboardUI ui)
    {
        _ui = ui;

        _questWindow.SetActive(true);

        _windowTitle.text = Game.Data.QuestStats.Quests[name].Title.ToUpper();
        _questMessage.text = Game.Data.QuestStats.Quests[name].RawMessage;

        ResourcesInfo info = Game.Data.QuestStats.Quests[name].Resource;
        _questReward.text = $"{info.Amount} <sprite name={info.Type}>";

        UpdateQuest(name);
        UpdateButton(name, OnAccept, OnClaim);  

        void OnAccept()
        {
            _ui.AcceptQuest();
            _questButton.gameObject.SetActive(false);

            _isExpanded = false;
            ExpandPanel();

            QuestFieldUI field = Instantiate(_questFieldPrefab, _questPanel.transform);
            field.Initialize(name);
            _questFields.Add(name, field);

            BaseQuest quest = Quest.GetQuest(name);
            Game.Data.RuntimeQuestStats.Quests.Add(name, quest);
            quest.OnAcceptQuest();

            UpdateQuest(name);
            UpdateButton(name, OnAccept, OnClaim);
        }

        void OnClaim()
        {
            _ui.ClaimReward();
            _questWindow.SetActive(false);

            Destroy(_questFields[name].gameObject);
            _questFields.Remove(name);
        }
    }

    private void UpdateQuest(string name)
    {
        if (Game.Data.RuntimeQuestStats.Quests.TryGetValue(name, out BaseQuest quest))
        {
            _questProgress.text = quest.GetProgress();
        }
    }

    private void UpdateButton(string name, UnityAction onAccept, UnityAction onClaim)
    {
        if (!Game.Data.RuntimeQuestStats.Quests.ContainsKey(name))
        {
            _questButton.gameObject.SetActive(true);
            _buttonText.text = "ACCEPT";
            _questButton.onClick.RemoveAllListeners();
            _questButton.onClick.AddListener(onAccept);
        }
        else if (Game.Data.RuntimeQuestStats.Quests[name].IsCompleted)
        {
            _questButton.gameObject.SetActive(true);
            _buttonText.text = "CLAIM";
            _questButton.onClick.RemoveAllListeners();
            _questButton.onClick.AddListener(onClaim);
        }
    }

    private void ExpandPanel()
    {
        _isExpanded = !_isExpanded;
        _questPanel.SetActive(_isExpanded);

        _expandingArrow.transform.localRotation = Quaternion.Euler(0, 0, _isExpanded ? 0 : 180);
    }
}