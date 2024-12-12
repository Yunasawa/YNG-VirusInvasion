using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Extensions.Methods;

public class FarmNodeUI : MonoBehaviour
{
    private PlayerStats _playerStats => Game.Data.PlayerStats;
    private ConstructStats _constructionStat => Game.Data.ConstructStats;
    private FarmStatsNode _node;

    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _buttonLabel;

    private (ResourceType Type, int Amount) _upgradeCost = new();
    private bool _enoughResource = false;

    private void Awake()
    {
        Player.OnChangeResources += UpdateNode;
    }

    private void OnDestroy()
    {
        Player.OnChangeResources -= UpdateNode;
    }

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
        PlayerStats.ExtraStats extraStats = _playerStats.FarmStats[Construct.CurrentConstruct][_node.Name];
        (float from, float to) stats = new(extraStats.Value.RoundToDigit(1), extraStats.NextValue.RoundToDigit(1));
        (string from, string to) statsToString = new($"<color=#E8FF15><b>{stats.from}</b></color>", $"<color=#E8FF15><b>{stats.to}</b></color>");
        _text.text = $"{_node.Description.ReplaceStats(statsToString.from, statsToString.to)}\nLevel: {extraStats.Level}";

        _upgradeCost = Formula.Upgrade.GetFarmUpgradeCost(_node, (int)extraStats.Level);

        _enoughResource = Game.Data.PlayerStats.Resources[_upgradeCost.Type] >= _upgradeCost.Amount;

        _buttonLabel.text = $"{_upgradeCost.Amount}\n{_upgradeCost.Type}";
        _buttonLabel.color = _enoughResource ? Color.white : Color.red;
        _button.interactable = _enoughResource;
    }

    private void OnButtonClicked()
    {
        if (!_enoughResource) return;

        Player.OnConsumeResources?.Invoke(_upgradeCost.Type, _upgradeCost.Amount);
        Player.OnFarmStatsLevelUp?.Invoke(_node.Name);
    }
}