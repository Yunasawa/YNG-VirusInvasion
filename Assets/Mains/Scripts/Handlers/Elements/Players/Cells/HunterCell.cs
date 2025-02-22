using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using YNL.Bases;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

public class HunterCell : MonoBehaviour
{
    private SerializableDictionary<string, PlayerStats.ExtraStats> _extraStatsLevel => Game.Data.PlayerStats.ExtraStatsLevel;

    private NavMeshAgent _agent;
    [SerializeField] private Enemy TargetEnemy;
    [SerializeField] private Transform Target;
    private EnemyPool _pool;
    private EnemyPool _defeatedPool;

    [SerializeField] private int _currentCapacity;

    private HunterCellStats _stats;
    [SerializeField] private bool _startDamaging = false;
    [SerializeField] private bool _returningHome = false;
    [SerializeField] private bool _isFullCapacity = false;

    private SerializableDictionary<ResourceType, int> _containedResources = new();
    [SerializeField] private DeliveryWindowUI _ui;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!_isFullCapacity)
        {
            if (!TargetEnemy.IsNull())
            {
                if (TargetEnemy.IsEnable)
                {
                    Target = TargetEnemy?.transform;
                    _agent.SetDestination(Target.transform.position);
                }
                else
                {
                    TargetEnemy = GetTargetEnemy();
                    Target = TargetEnemy?.transform;
                }
            }
            else
            {
                TargetEnemy = GetTargetEnemy();
                Target = TargetEnemy?.transform;
            }
            _returningHome = false;
        }
        else
        {
            Target = Game.Input.HomeBase.transform;
            _agent.SetDestination(Target.transform.position);
            _returningHome = true;
        }
        if (!Target.IsNull())
        {
            if (Vector3.Distance(transform.position, Target.position) <= _agent.stoppingDistance + 0.5f && !_startDamaging)
            {
                if (!_returningHome) _startDamaging = true;
                else
                {
                    foreach (var resource in _containedResources)
                    {
                        Game.Data.PlayerStats.AdjustResources(resource.Key, (int)resource.Value);
                    }

                    Player.OnChangeResources?.Invoke();

                    _currentCapacity = 0;
                    _isFullCapacity = false;
                    _containedResources.Clear();

                    _stats.Capacity = 0;
                    _stats.Resources.Clear();

                    _returningHome = false;

                    _ui.ClearResources();
                }
            }
            if (_startDamaging) TargetEnemy.Stats.GetHit();
        }

        _stats.Position = new(transform.position);

        Vector3 direction = Camera.main.transform.forward;
        Quaternion rotation = Quaternion.LookRotation(direction);
        _ui.transform.rotation = rotation;
        _ui.transform.localScale = Vector3.one * 0.003f * Camera.main.transform.localPosition.magnitude / 36;
    }

    public void Initialize(HunterCellStats stats)
    {
        _stats = stats;

        transform.position = stats.Position.ToVector3();

        TargetEnemy = GetTargetEnemy();
        Target = TargetEnemy?.transform;

        _currentCapacity = stats.Capacity;

        _containedResources = stats.Resources;
        _ui.UpdateResources(_containedResources);
    }

    public void OnDefeatEnemy()
    {
        _startDamaging = false;
        _defeatedPool = _pool;
        _pool = null;
        DisablePool().Forget();

        _currentCapacity += Game.Data.EnemySources[TargetEnemy.Stats.ID].Capacity;
        if (_currentCapacity >= _extraStatsLevel[Key.Stats.HunterCapacity].Value) _isFullCapacity = true;

        _stats.Capacity = _currentCapacity;

        EnemySources sources = Game.Data.EnemySources[TargetEnemy.Stats.ID];
        foreach (var resource in sources.Drops)
        {
            if (_containedResources.ContainsKey(resource.Key)) _containedResources[resource.Key] += (int)resource.Value;
            else _containedResources.Add(resource.Key, (int)resource.Value);
        }

        _stats.Resources = _containedResources;

        _ui.UpdateResources(_containedResources);

        async UniTaskVoid DisablePool()
        {
            await UniTask.WaitForSeconds(1);

            if (_defeatedPool == _pool) return;

            _defeatedPool.gameObject.SetActive(false);
        }
    }

    private Enemy GetTargetEnemy()
    {
        if (Game.Enemy.ValidEnemies.IsNullOrEmpty()) return null;

        _pool = Game.Enemy.ValidEnemies.GetRandom();
        Enemy enemy = _pool.Enemies.FirstOrDefault(i => i.IsEnable && !i.IsCaught);

        if (!enemy.IsNull())
        {
            enemy.IsCaught = true;
            enemy.Movement.SetDamager(transform);
            enemy.OnEnemyKilled = OnDefeatEnemy;
            enemy.UI.UpdateHealthBar(enemy.IsCaught);
            enemy.Stats.SetDamage(_extraStatsLevel[Key.Stats.HunterDPS].Value);
            _pool.gameObject.SetActive(true);
            return enemy;
        }

        return GetTargetEnemy();
    }
}