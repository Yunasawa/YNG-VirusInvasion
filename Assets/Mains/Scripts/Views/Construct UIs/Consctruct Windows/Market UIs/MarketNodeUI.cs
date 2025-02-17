using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Extensions.Methods;

public class MarketNodeUI : MonoBehaviour
{
    private PlayerStats _playerStats => Game.Data.PlayerStats;
    private MarketStatsNode _node;

    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _buttonLabel;
    [SerializeField] private SwitchButton _button;

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
        _button.OnClick.AddListener(OnButtonClicked);
    }

    public void Initialize(MarketStatsNode node)
    {
        _node = node;
        UpdateNode();
    } 

    public void UpdateNode()
    {
        bool isMaxLevel = _playerStats.ExtraStatsLevel[_node.Key].Level >= Formula.Stats.GetMaxExtraLevel(_node.Key);
        _enoughResource = Game.Data.PlayerStats.Resources[_upgradeCost.Type] >= _upgradeCost.Amount;

        PlayerStats.ExtraStats extraStats = _playerStats.ExtraStatsLevel[_node.Key];
        (float from, float to) stats = new(extraStats.Value.RoundToDigit(1), extraStats.NextValue.RoundToDigit(1));
        (string from, string to) statsToString = new($"<color=#7D6C00>{stats.from}</color>", $"<color=#0C7B3E>{stats.to}</color>");

        string levelTitle = isMaxLevel ? "<color=#0058AF>  MAX LEVEL" : $"<color=#455463>  Level <color=#0058AF>{extraStats.Level}</color> > <color=#0058AF>{extraStats.Level + 1}</color>";
        string upgradeTitle = $"<size=40><color=#D9393E>{_node.Key.AddSpaces()}</color></size>{levelTitle}</color>\n";

        _text.text = upgradeTitle + $"{_node.Description.Replace("$1", statsToString.from).Replace("$2", statsToString.to)}";

        if (isMaxLevel)
        {
            _buttonLabel.text = "<size=30>MAX LEVEL</size>";
            _enoughResource = false;
        }
        else
        {
            _upgradeCost = Formula.Upgrade.GetMarketUpgradeCost(_node, (int)extraStats.Level);
            _buttonLabel.text = $"{_upgradeCost.Amount} <sprite name={_upgradeCost.Type}>";
        }
        
        EnableInteraction();

        if (Game.Data.Vault.MarketIcons.TryGetValue(_node.Key, out Sprite icon))
        {
            _icon.sprite = icon;
        }
    }

    private void EnableInteraction()
    {
        _buttonLabel.color = _enoughResource ? Color.white : Color.red;
        _button.Graphic.color = _enoughResource ? Color.white : "#FFFFFF80".ToColor();
    }

    private void OnButtonClicked()
    {
        if (!_enoughResource) return;

        Player.OnConsumeResources?.Invoke(_upgradeCost.Type, _upgradeCost.Amount);
        Player.OnExtraStatsLevelUp?.Invoke(_node.Key);
        Player.OnTradeInMarket?.Invoke(Player.Construction.Construct.Name);
    }
}