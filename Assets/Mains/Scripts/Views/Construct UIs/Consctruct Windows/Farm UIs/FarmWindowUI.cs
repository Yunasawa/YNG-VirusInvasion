using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

public class FarmWindowUI : ConstructWindowUI
{
    private RuntimeConstructStats _runtimeConstructStats => Game.Data.RuntimeStats.ConstructStats;
    private RuntimeFarmStats _farmStats => _runtimeConstructStats.Farms[Construct.CurrentConstruct];

    private FarmStats _stats;
    private FarmConstruct _farm;

    [SerializeField] private FarmNodeUI _nodePrefab;
    [SerializeField] private Transform _nodeContainer;

    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _capacityText;
    [SerializeField] private Image _capacityBar;
    [SerializeField] private Button _collectButton;

    [SerializeField] private SerializableDictionary<ResourceType, ResourceNodeUI> _resourceNodes = new();

    private Dictionary<string, FarmNodeUI> _nodes = new();
    private bool _isWindowOpening = false;

    private void Awake()
    {
        _collectButton.onClick.AddListener(CollectResouce);

        Player.OnFarmStatsUpdate += OnFarmStatsUpdate;
        Player.OnChangeResources += UpdateResourceNodes;
        Player.OnFarmStatsLevelUp += OnFarmStatsLevelUp;

        Construct.OnFarmGenerateResource += OnFarmGenerateResource;
        Construct.OnFarmCountdown += OnFarmCountdown;
    }

    private void OnDestroy()
    {
        Player.OnFarmStatsUpdate -= OnFarmStatsUpdate;
        Player.OnChangeResources -= UpdateResourceNodes;
        Player.OnFarmStatsLevelUp -= OnFarmStatsLevelUp;

        Construct.OnFarmGenerateResource -= OnFarmGenerateResource;
        Construct.OnFarmCountdown -= OnFarmCountdown;
    }

    public override void OnOpenWindow()
    {
        _farm = Player.Construction.Construct.GetComponent<FarmConstruct>();

        _title.text = Game.Data.ConstructStats.Farms[Construct.CurrentConstruct.Substring(0, 5)].Name;
        _icon.sprite = Game.Data.Vault.ResourceIcons[_farm.GeneratedResource];
        CreateNodes();

        Construct.OnFarmCountdown?.Invoke(_farm, _farm.TimeCounter);
        UpdateResourceNodes();
        UpdateCapacityStatus();

        _isWindowOpening = true;
    }

    public override void OnCloseWindow()
    {
        _isWindowOpening = false;
    }
    private void CreateNodes()
    {
        _stats = Game.Data.ConstructStats.Farms[Construct.CurrentConstruct.Substring(0, 5)];

        _nodeContainer.DestroyAllChildren();

        foreach (var stat in _stats.Nodes)
        {
            var node = Instantiate(_nodePrefab, _nodeContainer);
            node.Initialize(stat);

            if (!_nodes.ContainsKey(stat.Name)) _nodes.Add(stat.Name, node);
        }
    }
    private void OnFarmStatsUpdate(string key)
    {
        if (_nodes.ContainsKey(key)) _nodes[key].UpdateNode();
    }
    private void OnFarmStatsLevelUp(string key)
    {
        if (_nodes.ContainsKey(key))
        {
            UpdateCapacityStatus();
            UpdateResourceNodes();
            OnFarmStatsUpdate(key);
        }
    }
    private void CollectResouce()
    {
        _farm.CollectResource();

        UpdateCapacityStatus();
    }
    private void UpdateResourceNodes()
    {
        foreach (var pair in _resourceNodes) pair.Value.UpdateNode(Game.Data.PlayerStats.Resources[pair.Key]);
    }
    private void UpdateCapacityStatus()
    {
        if (_farm.Capacity != 0)
        {
            _capacityBar.fillAmount = _farmStats.Current / _farm.Capacity;
            _capacityText.text = $"{_farmStats.Current.RoundToDigit(1)}/{_farm.Capacity}";
        }
        else
        {
            _capacityBar.fillAmount = 0;
            _capacityText.text = $"No capacity";
        }
    }

    private void OnFarmGenerateResource(FarmConstruct farm)
    {
        if (farm != _farm || !_isWindowOpening) return;

        UpdateCapacityStatus();
    }

    private void OnFarmCountdown(FarmConstruct farm, int time)
    {
        _timerText.text = $"{time}s";
    }
}