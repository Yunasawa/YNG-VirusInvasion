using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;

public class FarmNodeUI : MonoBehaviour
{
    private PlayerStats _playerStats => Game.Data.PlayerStats;
    private ConstructStats _constructionStat => Game.Data.ConstructStats;
    private FarmStatsNode _node;

    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _buttonLabel;

    private void Start()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    public void Initialize(FarmStatsNode node)
    {
        _node = node;
        UpdateNode();
    }

    public void UpdateNode()
    {
        PlayerStats.ExtraStats extraStats = _playerStats.FarmStatsLevel[_node.Name];
        (float from, float to) stats = new(extraStats.Value, extraStats.NextValue);
        (string from, string to) statsToString = new($"<color=#E8FF15><b>{stats.from}</b></color>", $"<color=#E8FF15><b>{stats.to}</b></color>");
        _text.text = $"{_node.Description.ReplaceStats(statsToString.from, statsToString.to)}\nLevel: {extraStats.Level}";
    }

    private void OnButtonClicked()
    {
        Player.OnFarmStatsLevelUp?.Invoke(_node.Name);
    }
}