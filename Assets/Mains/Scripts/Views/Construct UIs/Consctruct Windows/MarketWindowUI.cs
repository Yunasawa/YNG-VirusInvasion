using System;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;

public class MarketWindowUI : MonoBehaviour
{
    private MarketStats _stats;

    [SerializeField] private MarketNodeUI _nodePrefab;
    [SerializeField] private Transform _nodeContainer;

    private void Start()
    {
        if (Player.Construction.Construct.Type != ConstructType.Market) return;

        Initialize();
    }

    private void Initialize()
    {
        _stats = Array.Find(Game.Data.ConstructStats.Markets, x => x.Name == Player.Construction.Construct.Name);

        _nodeContainer.DestroyAllChildren();

        foreach (var stat in _stats.Nodes)
        {
            var node = Instantiate(_nodePrefab, _nodeContainer);
            node.Initialize(stat);
        }
    }
}