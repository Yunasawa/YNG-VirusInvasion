using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Extensions.Methods;

public class FarmNodeUI : MonoBehaviour
{
    private RuntimeConstructStats _runtimeConstructStats => Game.Data.RuntimeStats.ConstructStats;

    private FarmStatsNode _node;

    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private SwitchButton _button;
    [SerializeField] private TextMeshProUGUI _buttonLabel;

    private string _constructKey;
    private string _nodeName;

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

    public void Initialize(string key, string nodeName, FarmStatsNode node)
    {
        _constructKey = key;
        _nodeName = nodeName;
        _node = node;
        UpdateNode();
    }

    public void UpdateNode()
    {
        if (this.IsNullOrDestroyed()) return;
        if (!this.gameObject.activeInHierarchy) return;

        if (_node == null) _node = Game.Data.ConstructStats.Farms[_constructKey].Nodes.First(i => i.Name == _nodeName);

        RuntimeFarmStats.Attribute extraStats = _runtimeConstructStats.Farms[Construct.CurrentConstruct].Attributes[_node.Name];

        _icon.sprite = Game.Data.Vault.FarmIcons[_node.Name];

        (float from, float to) stats = new(extraStats.Value.RoundToDigit(1), extraStats.NextValue.RoundToDigit(1));
        (string from, string to) statsToString = new($"<color=#7D6C00><b>{stats.from}</b></color>", $"<color=#0C7B3E><b>{stats.to}</b></color>");

        string content = $"<size=45><color=#D9393E>{_node.Name}</color></size>: Level <color=#0058AF>{extraStats.Level}</color> > <color=#0058AF>{extraStats.Level + 1}</color>\n";
        content += $"{_node.Description.Replace("$1", statsToString.from).Replace("$2", statsToString.to)}";

        _text.text = content;

        _upgradeCost = Formula.Upgrade.GetFarmUpgradeCost(_node, (int)extraStats.Level);

        _enoughResource = Game.Data.PlayerStats.Resources[_upgradeCost.Type] >= _upgradeCost.Amount;

        _buttonLabel.text = $"{_upgradeCost.Amount} <sprite name={_upgradeCost.Type}>";

        EnableInteraction();
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
        Player.OnFarmStatsLevelUp?.Invoke(_node.Name);
    }
}