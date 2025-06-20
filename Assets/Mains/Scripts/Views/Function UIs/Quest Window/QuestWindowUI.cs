﻿using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Extensions.Methods;
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
    [SerializeField] private GameObject _progressBox;
    [SerializeField] private TextMeshProUGUI _questReward;
    [SerializeField] private Button _questButton;
    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private Image _questIcon;

    [Title("Quest Advance")]
    [SerializeField] private GameObject _normalWindow;
    [SerializeField] private GameObject _advanceWindow;
    [SerializeField] private TextMeshProUGUI _advanceMessage;
    [SerializeField] private SerializableDictionary<string, Image> _advanceIcons = new();

    private QuestBillboardUI _ui;

    private void Awake()
    {
        Player.OnOpenQuestWindow += OnOpenQuestWindow;
        View.OnCloseQuestWindow += CloseWindow;
        Quest.OnDeserializeQuest += OnDeserializeQuest;
        Quest.OnUpdateQuestStatus += OnUpdateQuestStatus;

        _expandingButton.onClick.AddListener(ExpandPanel);
        _closeButton.onClick.AddListener(CloseWindow);
    }

    private void OnDestroy()
    {
        Player.OnOpenQuestWindow -= OnOpenQuestWindow;
        View.OnCloseQuestWindow -= CloseWindow;
        Quest.OnDeserializeQuest -= OnDeserializeQuest;
        Quest.OnUpdateQuestStatus += OnUpdateQuestStatus;
    }

    private void Start()
    {
        _progressBox.SetActive(false);
        DisableQuestPanel().Forget();

        async UniTaskVoid DisableQuestPanel()
        {
            await UniTask.NextFrame();
            _questPanel.SetActive(false);
        }
    }

    private void OnDeserializeQuest(string name)
    {
        CreateQuestNodeUI(name);
    }

    private void OnUpdateQuestStatus(string name, string value)
    {
        if (name != "MainQuest2") UpdateNormalContent(name);
    }

    private void OnOpenQuestWindow(string name, QuestBillboardUI ui)
    {
        _ui = ui;

        _questWindow.SetActive(true);
        _windowTitle.text = Game.Data.QuestStats.Quests[name].Title.ToUpper();

        if (name != "MainQuest2") UpdateNormalContent(name);
        else UpdateAdvanceContent(name);

        UpdateQuest(name);
        UpdateButton(name, OnAccept, OnClaim);  

        void OnAccept()
        {
            _questButton.gameObject.SetActive(false);

            _isExpanded = false;
            ExpandPanel();

            BaseQuest quest = Quest.GetQuest(name);
            Game.Data.RuntimeQuestStats.Quests.Add(name, quest);
            quest.OnAcceptQuest();

            CreateQuestNodeUI(name);
            
            _ui.AcceptQuest();

            UpdateQuest(name);
            UpdateButton(name, OnAccept, OnClaim);
        }

        void OnClaim()
        {
            _ui.ClaimReward();
            _questWindow.SetActive(false);

            Destroy(_questFields[name].gameObject);
            _questFields.Remove(name);

            Game.Data.RuntimeQuestStats.CompletedQuests.Add(name);
            Quest.OnCompleteQuest?.Invoke(name);
        }
    }

    private void UpdateNormalContent(string name)
    {
        _normalWindow.SetActive(true);
        _advanceWindow.SetActive(false);

        QuestStatsNode quest = Game.Data.QuestStats.Quests[name];

        _questMessage.text = quest.Message.Replace("%", quest.Target);
        UpdateGeneralContent(name);

        _questIcon.sprite = Game.Data.Vault.QuestIcons[name];
    }
    private void UpdateAdvanceContent(string name)
    {
        _advanceWindow.SetActive(true);
        _normalWindow.SetActive(false);

        QuestStatsNode quest = Game.Data.QuestStats.Quests[name];

        _advanceMessage.text = quest.Message.Replace("%", quest.Target);
        UpdateGeneralContent(name);

        foreach (var pair in _advanceIcons)
        {
            bool completed = Game.Data.RuntimeQuestStats.CompletedQuests.Contains(pair.Key);

            pair.Value.color = completed ? "#FFFFFFFF".ToColor() : "#000000C0".ToColor();
        }
    }
    private void UpdateGeneralContent(string name)
    {
        List<ResourcesInfo> infos = Game.Data.QuestStats.Quests[name].Rewards;
        string reward = "";
        foreach (var info in infos)
        {
            reward += $"{info.Amount.RoundValue()}<sprite name={info.Type}>    ";
            if (infos.IsLast(info) && name.Contains("MiniQuest")) reward += $"1<sprite name={name}>";
        }
        _questReward.text = reward;
    }

    private void CreateQuestNodeUI(string name)
    {
        QuestFieldUI field = Instantiate(_questFieldPrefab, _questPanel.transform);
        field.Initialize(name);
        _questFields.Add(name, field);
    }

    private void UpdateQuest(string name)
    {
        if (Game.Data.RuntimeQuestStats.Quests.TryGetValue(name, out BaseQuest quest))
        {
            _progressBox.SetActive(true);
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
        else
        {
            _questButton.gameObject.SetActive(false);
        }
    }

    private void ExpandPanel()
    {
        _isExpanded = !_isExpanded;
        _questPanel.SetActive(_isExpanded);

        _expandingArrow.transform.localRotation = Quaternion.Euler(0, 0, _isExpanded ? 0 : 180);
    }

    private void CloseWindow()
    {
        _questWindow.SetActive(false);
    }
}