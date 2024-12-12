using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Utilities.Addons;

public class QuestWindowUI : MonoBehaviour
{
    [Title("Quest Field")]
    [SerializeField] private SerializableDictionary<string, QuestFieldUI> _questFields = new();

    [SerializeField] private Button _expandingButton;
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
        Player.OnLevelUp += Start;

        Player.OnFarmStatsUpdate += OnUpgradeFarmAttribute;
        Player.OnAcceptQuest += OnAcceptQuest;
        Player.OnCompleteQuest += OnCompleteQuest;

        Player.OnOpenQuestWindow += OnOpenQuestWindow;

        _expandingButton.onClick.AddListener(ExpandPanel);
        _closeButton.onClick.AddListener(() => _questWindow.SetActive(false));
    }

    private void OnDestroy()
    {
        Player.OnLevelUp -= Start;

        Player.OnFarmStatsUpdate -= OnUpgradeFarmAttribute;
        Player.OnAcceptQuest -= OnAcceptQuest;
        Player.OnCompleteQuest -= OnCompleteQuest;

        Player.OnOpenQuestWindow -= OnOpenQuestWindow;
    }

    private void Start()
    {
        foreach (var quest in _questFields) quest.Value.Initialize(0);

        _questPanel.SetActive(false);
    }

    private void OnOpenQuestWindow(string name, QuestBillboardUI ui)
    {
        _ui = ui;

        _questWindow.SetActive(true);

        int higher = Mathf.Max(Game.Data.PlayerStats.FarmStats["Farm1.5"]["Capacity"].Level, Game.Data.PlayerStats.FarmStats["Farm1.5"]["Income"].Level);

        _windowTitle.text = Game.Data.QuestStats.Quests[name].Title;
        _questMessage.text = Game.Data.QuestStats.Quests[name].RawMessage;
        _questProgress.text = $"{higher}/2";

        ResourcesInfo info = Game.Data.QuestStats.Quests[name].Resource;
        _questReward.text = $"{info.Amount}";

        if (!Game.Data.QuestRuntime.CurrentQuests.ContainsKey(name))
        {
            _questButton.gameObject.SetActive(true);
            _buttonText.text = "ACCEPT";
            _questButton.onClick.RemoveAllListeners();
            _questButton.onClick.AddListener(OnAccept);
        }
        else if (Game.Data.QuestRuntime.CurrentQuests[name])
        {
            _questButton.gameObject.SetActive(true);
            _buttonText.text = "CLAIM";
            _questButton.onClick.RemoveAllListeners();
            _questButton.onClick.AddListener(OnClaim);
        }

        void OnAccept()
        {
            _ui.AcceptQuest();
            _questButton.gameObject.SetActive(false);
        }

        void OnClaim()
        {
            _ui.ClaimReward();
            _questWindow.SetActive(false);
        }
    }

        private void ExpandPanel()
    {
        _isExpanded = !_isExpanded;
        _questPanel.SetActive(_isExpanded);
    }

    private void OnUpgradeFarmAttribute(string key = "")
    {
        int higher = Mathf.Max(Game.Data.PlayerStats.FarmStats["Farm1.5"]["Capacity"].Level, Game.Data.PlayerStats.FarmStats["Farm1.5"]["Income"].Level);
        _questFields["UpgradeFarm"].UpdateQuest(higher);
     
        if (higher >= 2) _questFields["UpgradeFarm"].ShowReward();
    }

    private void OnAcceptQuest(string name)
    {
        _questFields[name].gameObject.SetActive(true);
        OnUpgradeFarmAttribute();
    }

    private void OnCompleteQuest(string name)
    {
        _questFields[name].gameObject.SetActive(false);
    }
}