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
    private FarmStats _stats;

    [SerializeField] private FarmNodeUI _nodePrefab;
    [SerializeField] private Transform _nodeContainer;

    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _capacityText;
    [SerializeField] private Image _capacityBar;
    [SerializeField] private Button _collectButton;

    [SerializeField] private SerializableDictionary<ResourceType, ResourceNodeUI> _resourceNodes = new();

    private Dictionary<string, FarmNodeUI> _nodes = new();

    private float _statsValue;

    private int _timeCounter = 10;
    private bool _isWindowStillOpen = false;
    private bool _isTaskDone = true;
    private CancellationTokenSource _tokenSource;

    private (float ResourceAmount, int RemainTime) _resourceDelta;
    private FarmConstruct _farm;

    private int _timePoint = 10;

    private void Awake()
    {
        Player.OnFarmStatsUpdate += OnFarmStatsUpdate;
        View.OnUpdateResourceNodes += UpdateResourceNodes;
    }

    private void OnDestroy()
    {
        Player.OnFarmStatsUpdate -= OnFarmStatsUpdate;
        View.OnUpdateResourceNodes -= UpdateResourceNodes;
    }

    private void Start()
    {
        _collectButton.onClick.AddListener(CollectResouce);
    }

    public override void OnOpenWindow()
    {
        _statsValue = Game.Data.PlayerStats.FarmStatsLevel[$"{Player.Construction.Construct.Name.RemoveAll(" ")}Income"].Value;

        _farm = Player.Construction.Construct.GetComponent<FarmConstruct>();
        CreateNodes();

        _resourceDelta = CalculateResourceDelta();
        GenerateResource(_resourceDelta.ResourceAmount);
        _timeCounter -= _resourceDelta.RemainTime;
        StartCountingDown(true);

        UpdateResourceNodes();
    }

    public override void OnCloseWindow()
    {
        _farm.OnWindowClose();
        _farm.StartCountToPing(_timeCounter);
        StartCountingDown(false);
    }

    private void CreateNodes()
    {
        _stats = Array.Find(Game.Data.ConstructStats.Farms, x => x.Name == Player.Construction.Construct.Name);

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

    private void StartCountingDown(bool isStart)
    {
        _isWindowStillOpen = isStart;
        if (isStart)
        {
            if (_isTaskDone) _timeCounter -= _resourceDelta.RemainTime;
            _timerText.text = $"{_timeCounter}s";
            if (_isTaskDone)
            {
                CountDownTask().Forget();
                _isTaskDone = false;
            }
        }
    }

    private async UniTaskVoid CountDownTask()
    {
        while (true)
        {
            try
            {
                _timerText.text = $"{_timeCounter}s";
                await UniTask.WaitForSeconds(1);
                _timeCounter--;
                if (_timeCounter < 0)
                {
                    _timeCounter = _timePoint - 1;
                    GenerateResource(1);
                    if (!_isWindowStillOpen)
                    {
                        _farm.OnWindowClose();
                        _isTaskDone = true;
                        break;
                    }
                }
            }
            catch (OperationCanceledException) { break; }
        }
    }

    private (float, int) CalculateResourceDelta()
    {
        int deltaSecond = _farm.DeltaSecond();
        float resourceAmount = deltaSecond / _timePoint;
        int remainSecond = deltaSecond % _timePoint;
        return new(resourceAmount, remainSecond);
    }

    private void GenerateResource(float amount)
    {
        float resources = amount * _statsValue;
    }

    private void CollectResouce()
    {
        if (true) _farm.ResourcePing.SetActive(false);
    }

    private void UpdateResourceNodes()
    {
        foreach (var pair in _resourceNodes) pair.Value.UpdateNode(Game.Data.PlayerStats.Resources[pair.Key]);
    }
}