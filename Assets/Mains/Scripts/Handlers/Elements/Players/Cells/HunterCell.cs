using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using YNL.Extensions.Methods;

public class HunterCell : MonoBehaviour
{
    private NavMeshAgent _agent;
    [SerializeField] private Enemy Target;

    private List<Group<string, int>> _defeatedEnemies = new();

    private HunterCellStats _stats;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!Target.IsNull()) _agent.SetDestination(Target.transform.position);
        else Target = GetTargetEnemy();
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

        EnemyPool pool = Game.Enemy.ValidEnemies.GetRandom();
        Enemy enemy = pool.Enemies.FirstOrDefault(i => i.IsEnable);

        if (!enemy.IsNull()) return enemy;
        
        return GetTargetEnemy();
    }
}