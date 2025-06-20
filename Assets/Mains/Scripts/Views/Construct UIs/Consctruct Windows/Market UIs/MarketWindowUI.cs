using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

public class MarketWindowUI : ConstructWindowUI
{
    private MarketStats _stats;
    private MarketConstruct _construct;

    [SerializeField] private MarketNodeUI _nodePrefab;
    [SerializeField] private Transform _nodeContainer;

    [SerializeField] private SerializableDictionary<ResourceType, ResourceNodeUI> _resourceNodes = new();

    private Dictionary<string, MarketNodeUI> _nodes = new();

    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _evolutionInfo; 
    [SerializeField] private Button _evolutionButton; 
    [SerializeField] private TextMeshProUGUI _evolutionCost;

    private bool _ableToEvolute = true;
    private MarketEvolutionNode _evolution;

    private void Awake()
    {
        Player.OnExtraStatsUpdate += OnExtraStatsUpdate;
        Player.OnChangeResources += UpdateResourceNodes;
        Player.OnChangeResources += UpdateResourceNodes;

        Cheat.OnEvolution += A;

        _evolutionButton.onClick.AddListener(() => UpdateEvolution(false));
    }

    private void OnDestroy()
    {
        Player.OnExtraStatsUpdate -= OnExtraStatsUpdate;
        Player.OnChangeResources -= UpdateResourceNodes;
        Player.OnChangeResources += UpdateResourceNodes;

        Cheat.OnEvolution -= A;
    }

    private void Start()
    {
        this.gameObject.SetActive(false);   
    }

    public override void OnOpenWindow()
    {
        Initialize();

        UpdateResourceNodes();
    }

    private void Initialize()
    {
        _construct = Player.Construction.Construct.GetComponent<MarketConstruct>();
        _stats = Game.Data.ConstructStats.Markets[Player.Construction.Construct.Name];

       // _title.text = _stats.Name;
        //MDebug.Log($"{_title.text} - {_stats.Name}");

        _evolution = _stats.Evolutions.FirstOrDefault(i => i.Evolution == _construct.Evolution);

        _evolutionInfo.text = _evolution.IsNull() ? $"MAX EVOLUTION" : $"Evolution <color=#8E2300>{_construct.Evolution}</color> > <color=#8E2300>{_construct.Evolution + 1}</color>: DPS + 20%";
        _evolutionButton.gameObject.SetActive(!_evolution.IsNull());

        _ableToEvolute = true;

        _evolutionCost.text = "";

        if (!_evolution.IsNull())
        {
            foreach (var cost in _evolution.Costs)
            {
                if (Game.Data.PlayerStats.Resources[cost.Type] >= cost.Amount) _evolutionCost.text += $"{cost.Amount} <sprite name={cost.Type}> ";
                else
                {
                    _evolutionCost.text += $"<color=#FF0000>{cost.Amount} <sprite name={cost.Type}></color> ";
                    _ableToEvolute = false;
                }
            }
        }

        _nodeContainer.DestroyAllChildren();

        foreach (var stat in _stats.Nodes)
        {
            if (stat.Evolution > _construct.Evolution) continue;

            var node = Instantiate(_nodePrefab, _nodeContainer);
            node.Initialize(stat);

            if (!_nodes.ContainsKey(stat.Key)) _nodes.Add(stat.Key, node);
        }


    }

    private void OnExtraStatsUpdate(string key)
    {
        if (_nodes.ContainsKey(key)) _nodes[key].UpdateNode();
    }

    private void UpdateResourceNodes()
    {
        foreach (var pair in _resourceNodes) pair.Value.UpdateValue(Game.Data.PlayerStats.Resources[pair.Key]);
    }

    private void A() => UpdateEvolution(true);

    private void UpdateEvolution(bool force = false)
    {
        if (!_ableToEvolute && !force) return;
        Player.OnUpgradeRevolution?.Invoke();

        if (!force)
        {
            foreach (var cost in _evolution.Costs) Player.OnConsumeResources?.Invoke(cost.Type, cost.Amount);
            Game.Data.PlayerStats.Bonuses[AttributeType.DPS] += 20;
            Player.OnChangeStats?.Invoke();
            Initialize();
        }
    }
}