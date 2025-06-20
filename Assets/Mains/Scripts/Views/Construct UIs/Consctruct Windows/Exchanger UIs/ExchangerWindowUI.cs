using System;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private TextMeshProUGUI _title;
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
        _stats = Game.Data.ConstructStats.Exchangers[Player.Construction.Construct.Name];
        _title.text = _stats.Name;

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
                node.UpdateValue(Game.Data.PlayerStats.Resources[type]);
                node.UpdateIcon(type);
                _resourceNodes.Add(type, node);
            }
        }
        else UpdateResourceNodes();
    }

    private void UpdateResourceNodes()
    {
        foreach (var pair in _resourceNodes)
        {
            pair.Value.UpdateValue(Game.Data.PlayerStats.Resources[pair.Key]);
        }
    }
}