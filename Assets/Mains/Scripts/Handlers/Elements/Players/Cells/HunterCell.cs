using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using YNL.Bases;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

public class HunterCell : MonoBehaviour
{
    private SerializableDictionary<string, PlayerStats.ExtraStats> _extraStatsLevel => Game.Data.PlayerStats.ExtraStatsLevel;

    private NavMeshAgent _agent;
    [SerializeField] private Enemy Target;
    private EnemyPool _pool;

    private List<Group<string, int>> _defeatedEnemies = new();

    private HunterCellStats _stats;
    private bool _startDamaging = false;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!Target.IsNull())
        {
            if (Target.IsEnable) _agent.SetDestination(Target.transform.position);
            else Target = GetTargetEnemy();
        }
        else Target = GetTargetEnemy();

        if (!Target.IsNull())
        {
            if (Vector3.Distance(transform.position, Target.transform.position) <= _agent.stoppingDistance + 0.5f && !_startDamaging)
            {
                _startDamaging = true;
            }
            if (_startDamaging) Target.Stats.GetHit();
        }

        _stats.Position = new(transform.position);
    }

    public void Initialize(HunterCellStats stats)
    {
        _stats = stats;

        transform.position = stats.Position.ToVector3();

        Target = GetTargetEnemy();
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
            enemy.UI.UpdateHealthBar(enemy.IsCaught);
            enemy.Stats.SetDamage(_extraStatsLevel[Key.Stats.HunterDPS].Value);
            _pool.gameObject.SetActive(true);
            return enemy;
        }

        return GetTargetEnemy();
    }
}