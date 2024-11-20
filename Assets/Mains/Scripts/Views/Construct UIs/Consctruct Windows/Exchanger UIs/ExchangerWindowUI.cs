using System;
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

    private void Awake()
    {
        View.OnUpdateResourceNodes += UpdateResourceNodes;
    }

    private void OnDestroy()
    {
        View.OnUpdateResourceNodes -= UpdateResourceNodes;
    }

    public override void OnOpenWindow()
    {
        Initialize();
        UpdateResourceNodes();
    }

    private void Initialize()
    {
        _stats = Array.Find(Game.Data.ConstructStats.Exchangers, x => x.Name == Player.Construction.Construct.Name);

        _nodeContainer.DestroyAllChildren();

        foreach (var stat in _stats.Nodes)
        {
            var node = Instantiate(_nodePrefab, _nodeContainer);
            node.Initialize(stat);
        }
    }

    private void UpdateResourceNodes()
    {
        foreach (var pair in _resourceNodes)
        {
            pair.Value.UpdateNode(Game.Data.PlayerStats.Resources[pair.Key]);
        }
    }
}