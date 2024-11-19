using System;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;

public class ExchangerWindowUI : ConstructWindowUI
{
    private ExchangerStats _stats;

    [SerializeField] private ExchangerNodeUI _nodePrefab;
    [SerializeField] private Transform _nodeContainer;

    //private void Start()
    //{
    //    if (Player.Construction.Construct.Type != ConstructType.Exchanger) return;

    //    Initialize();
    //}

    public override void OnOpenWindow()
    {
        Initialize();
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
}