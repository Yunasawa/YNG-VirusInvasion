using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;

public class MarketNodeUI : MonoBehaviour
{
    private PlayerStats _playerStats => Game.Data.PlayerStats;
    private ConstructStats _constructionStat => Game.Data.ConstructStats;

    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _buttonLabel;
    [SerializeField] private Button _button;

    public void Initialize(MarketStatsNode node)
    {
        (float from, float to) stats = new(_playerStats.ExtraStatsLevel[node.Key].Value, _playerStats.ExtraStatsLevel[node.Key].NextValue);
        _text.text = $"{node.Name}\n{node.Description.ReplaceStats(stats.from, stats.to)}\nLevel: {_playerStats.ExtraStatsLevel[node.Key].Level}";
    }

    private void OnButtonClicked()
    {
        
    }
}