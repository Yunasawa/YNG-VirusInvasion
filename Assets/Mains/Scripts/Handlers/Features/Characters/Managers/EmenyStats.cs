using DG.Tweening;
using System.Linq;
using UnityEngine;
using YNL.Extensions.Methods;

public class EmenyStats : MonoBehaviour
{
    private Enemy _manager;

    public uint ID;

    public MonsterStatsNode Stats;
    public uint CurrentHealth;

    private void Awake()
    {
        _manager = GetComponent<Enemy>();
    }

    private void Start()
    {
        Initialization();
    }

    public void Initialization()
    {
        CurrentHealth = Stats.HP;
    }

    public void OnKilled()
    {
        _manager.OnEnemyKilled?.Invoke();
        _manager.OnEnemyKilled = null;
        Player.OnCollectEnemyDrops?.Invoke(Stats.Drops.Select(pair => (pair.Key, pair.Value)).ToArray());
    }
}