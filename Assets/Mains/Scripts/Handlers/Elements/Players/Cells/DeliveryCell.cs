using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using YNL.Bases;

public class DeliveryCell : MonoBehaviour
{
    private NavMeshAgent _agent;
    private bool _targetIsHome = false;
    private Vector3 _destination => _targetIsHome ? Game.Input.HomeBase.position : Player.Transform.position;
    private int _distance => _targetIsHome ? 10 : 2;
    private int _capacity => (int)Game.Data.PlayerStats.ExtraStatsLevel[Key.Stats.DeliveryCapacity].Value;
    private List<Group<string, int>> _deliveriedEnemies = new();
    public List<Group<string, int>> DeliveriedEnemies => _deliveriedEnemies;

    private DeliveryCellStats _stats;

    private Dictionary<ResourceType, int> _containedResources = new();
    [SerializeField] private DeliveryWindowUI _ui;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        _agent.SetDestination(_destination);

        if (Vector3.Distance(transform.position, _destination) <= _distance) HandleResources();

        _stats.Position = new(transform.position);

        Vector3 direction = Camera.main.transform.forward;
        Quaternion rotation = Quaternion.LookRotation(direction);

        _ui.transform.rotation = rotation;
        _ui.transform.localScale = Vector3.one * 0.003f * Camera.main.transform.localPosition.magnitude / 36;
    }

    public void Initialize(DeliveryCellStats stats)
    {
        _stats = stats;

        transform.position = stats.Position.ToVector3();
        _targetIsHome = stats.TargetIsHome;
        _deliveriedEnemies = stats.DeliveriedEnemies;
    }

    private void HandleResources()
    {
        if (_targetIsHome)
        {
            foreach (var pair in _deliveriedEnemies)
            {
                foreach (var resource in Game.Data.EnemySources[pair.E1].Drops)
                {
                    Game.Data.PlayerStats.AdjustResources(resource.Key, (int)resource.Value * pair.E2);
                }
            }

            Player.OnChangeResources?.Invoke();

            _deliveriedEnemies.Clear();
            _stats.DeliveriedEnemies = _deliveriedEnemies;

            _ui.ClearResources();
        }
        else
        {
            _deliveriedEnemies = Player.Enemy.GetValidEnemies(_capacity);
            _stats.DeliveriedEnemies = _deliveriedEnemies;

            if (_deliveriedEnemies.Count <= 0) return;

            _containedResources.Clear();
            foreach (var enemy in _deliveriedEnemies)
            {
                EnemySources sources = Game.Data.EnemySources[enemy.E1];
                foreach (var resource in sources.Drops)
                {
                    if (_containedResources.ContainsKey(resource.Key)) _containedResources[resource.Key] += (int)resource.Value;
                    else _containedResources.Add(resource.Key, (int)resource.Value);
                }
            }

            _ui.UpdateResources(_containedResources);
        }
        
        _targetIsHome = !_targetIsHome;
        _stats.TargetIsHome = _targetIsHome;
    }
}