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

    private float _statsValue => Game.Data.PlayerStats.FarmStats[Construct.CurrentConstruct]["Income"].Value;

    private int _timeCounter = Key.Config.FarmCountdown;
    private bool _isWindowStillOpen = false;
    private bool _isTaskDone = true;
    private CancellationTokenSource _tokenSource;

    private (float ResourceAmount, int RemainTime) _resourceDelta;
    private FarmConstruct _farm;

    private int _timePoint = Key.Config.FarmCountdown;

    private void Awake()
    {
        Player.OnFarmStatsUpdate += OnFarmStatsUpdate;
        Player.OnChangeResources += UpdateResourceNodes;
        Player.OnFarmStatsLevelUp += OnFarmStatsLevelUp;
    }

    private void OnDestroy()
    {
        Player.OnFarmStatsUpdate -= OnFarmStatsUpdate;
        Player.OnChangeResources -= UpdateResourceNodes;
        Player.OnFarmStatsLevelUp -= OnFarmStatsLevelUp;
    }

    private void Start()
    {
        _collectButton.onClick.AddListener(CollectResouce);
    }

    public override void OnOpenWindow()
    {
        _farm = Player.Construction.Construct.GetComponent<FarmConstruct>();
        CreateNodes();

        _resourceDelta = CalculateResourceDelta();
        GenerateResource(_resourceDelta.ResourceAmount);
        _timeCounter -= _resourceDelta.RemainTime;
        StartCountingDown(true);

        UpdateResourceNodes();
        UpdateCapacityStatus();
    }

    public override void OnCloseWindow()
    {
        _farm.OnWindowClose();
        _farm.CountToPingTask(_timeCounter).Forget();
        StartCountingDown(false);
    }
    private void CreateNodes()
    {
        _stats = Array.Find(Game.Data.ConstructStats.Farms, x => x.Name == Construct.CurrentConstruct.Substring(0, 5));

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
        }
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
        if (_farm.CurrentResources >= _farm.Capacity) return;

        _farm.CurrentResources += amount * _statsValue;
        _farm.CurrentResources.RefLimit(0, _farm.Capacity);

        UpdateCapacityStatus();
    }
    private void CollectResouce()
    {
        //if (_farm.CurrentResources > 0) _farm.ResourcePing.SetActive(false);

        Player.OnCollectFarmResources?.Invoke(_farm.GeneratedResource, _farm.CurrentResources);

        _farm.CurrentResources = 0;

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
            _capacityBar.fillAmount = _farm.CurrentResources / _farm.Capacity;
            _capacityText.text = $"{_farm.CurrentResources.RoundToDigit(1)}/{_farm.Capacity}";
        }
        else
        {
            _capacityBar.fillAmount = 0;
            _capacityText.text = $"No capacity";
        }
    }
}