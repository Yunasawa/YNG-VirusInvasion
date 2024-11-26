using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;

public class MarketNodeUI : MonoBehaviour
{
    private PlayerStats _playerStats => Game.Data.PlayerStats;
    private ConstructStats _constructionStat => Game.Data.ConstructStats;
    private MarketStatsNode _node;

    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _buttonLabel;
    [SerializeField] private Button _button;

    private void Start()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    public void Initialize(MarketStatsNode node)
    {
        _node = node;
        UpdateNode();
    } 

    public void UpdateNode()
    {
        PlayerStats.ExtraStats extraStats = _playerStats.ExtraStatsLevel[_node.Key];
        (float from, float to) stats = new(extraStats.Value, extraStats.NextValue);
        (string from, string to) statsToString = new($"<color=#E8FF15><b>{stats.from}</b></color>", $"<color=#E8FF15><b>{stats.to}</b></color>");
        _text.text = $"{_node.Description.ReplaceStats(statsToString.from, statsToString.to)}\nLevel: {extraStats.Level}";
    }

    private void OnButtonClicked()
    {
        Player.OnExtraStatsLevelUp?.Invoke(_node.Key);
    }
}