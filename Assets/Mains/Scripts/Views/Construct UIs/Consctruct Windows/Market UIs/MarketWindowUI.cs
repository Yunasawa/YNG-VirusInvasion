using System;
using System.Collections.Generic;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

public class MarketWindowUI : ConstructWindowUI
{
    private MarketStats _stats;

    [SerializeField] private MarketNodeUI _nodePrefab;
    [SerializeField] private Transform _nodeContainer;

    [SerializeField] private SerializableDictionary<ResourceType, ResourceNodeUI> _resourceNodes = new();

    private Dictionary<string, MarketNodeUI> _nodes = new();

    private void Awake()
    {
        Player.OnExtraStatsUpdate += OnExtraStatsUpdate;
        Player.OnChangeResources += UpdateResourceNodes;
        Player.OnChangeResources += UpdateResourceNodes;
    }

    private void OnDestroy()
    {
        Player.OnExtraStatsUpdate -= OnExtraStatsUpdate;
        Player.OnChangeResources -= UpdateResourceNodes;
        Player.OnChangeResources += UpdateResourceNodes;
    }

    public override void OnOpenWindow()
    {
        Initialize();

        UpdateResourceNodes();
    }

    private void Initialize()
    {
        _stats = Array.Find(Game.Data.ConstructStats.Markets, x => x.Name == Player.Construction.Construct.Name);

        _nodeContainer.DestroyAllChildren();

        foreach (var stat in _stats.Nodes)
        {
            if (stat.Evolution > Player.Construction.Construct.Evolution) continue;

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
        foreach (var pair in _resourceNodes) pair.Value.UpdateNode(Game.Data.PlayerStats.Resources[pair.Key]);
    }
}