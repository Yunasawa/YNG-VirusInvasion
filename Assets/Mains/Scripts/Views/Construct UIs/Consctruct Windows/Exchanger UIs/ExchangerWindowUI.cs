using System;
using System.Collections.Generic;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

public class ExchangerWindowUI : ConstructWindowUI
{
    private ExchangerStats _stats;

    [SerializeField] private ExchangerNodeUI _nodePrefab;
    [SerializeField] private Transform _nodeContainer;

    [SerializeField] private SerializableDictionary<ResourceType, ResourceNodeUI> _resourceNodes = new();
    [SerializeField] private ResourceNodeUI _resourceNode;
    [SerializeField] private Transform _resourceContainer;
    private List<ResourceType> _resourceTypes = new();
    private bool _isCreate = false;

    private void Awake()
    {
        Player.OnChangeResources += UpdateResourceNodes;
        Player.OnChangeResources += UpdateResourceNodes;
    }

    private void OnDestroy()
    {
        Player.OnChangeResources -= UpdateResourceNodes;
        Player.OnChangeResources -= UpdateResourceNodes;
    }

    public override void OnOpenWindow()
    {
        CreateNodes();
    }

    private void CreateNodes()
    {
        _stats = Array.Find(Game.Data.ConstructStats.Exchangers, x => x.Name == Player.Construction.Construct.Name);

        _resourceTypes.Clear();
        _nodeContainer.DestroyAllChildren();

        foreach (var stat in _stats.Nodes)
        {
            var node = Instantiate(_nodePrefab, _nodeContainer);
            node.Initialize(stat);

            if (!_resourceTypes.Contains(stat.From.Type)) _resourceTypes.Add(stat.From.Type);
            if (!_resourceTypes.Contains(stat.To.Type)) _resourceTypes.Add(stat.To.Type);
        }

        if (!_isCreate)
        {
            _resourceNodes.Clear();
            _resourceContainer.DestroyAllChildren();

            foreach (var type in _resourceTypes)
            {
                var node = Instantiate(_resourceNode, _resourceContainer);
                node.UpdateNode(Game.Data.PlayerStats.Resources[type]);
                _resourceNodes.Add(type, node);
            }
        }
        else UpdateResourceNodes();
    }

    private void UpdateResourceNodes()
    {
        foreach (var pair in _resourceNodes)
        {
            pair.Value.UpdateNode(Game.Data.PlayerStats.Resources[pair.Key]);
        }
    }
}