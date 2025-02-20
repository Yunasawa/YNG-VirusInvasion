using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

[AddComponentMenu("Game: Enemy")]
public class GameEnemy : MonoBehaviour
{
    private SerializableDictionary<string, PlayerStats.ExtraStats> _extraStatsLevel => Game.Data.PlayerStats.ExtraStatsLevel;
    private SerializableDictionary<string, EnemySources> _enemySources => Game.Data.EnemySources;

    private List<EnemyPool> _enemyPools = new();
    public List<EnemyPool> ValidEnemies = new();

    public void MonoAwake()
    {
        _enemyPools.Clear();

        Player.OnExtraStatsUpdate += OnUpgradeMiniStats;
    }

    private void OnDestroy()
    {
        Player.OnExtraStatsUpdate -= OnUpgradeMiniStats;
        
    }

    private void Start()
    {
        DelayInitialize().Forget();
    }

    private async UniTaskVoid DelayInitialize()
    {
        await UniTask.NextFrame();

        Player.OnExtraStatsUpdate?.Invoke(Key.Stats.HunterDPS);
    }

    public void RegisterEnemyPool(EnemyPool pool)
    {
        _enemyPools.Add(pool);
    }

    public void OnUpgradeMiniStats(string stats)
    {
        if (stats != Key.Stats.HunterDPS) return;

        ValidEnemies.Clear();
        float hunterDPS = _extraStatsLevel[Key.Stats.HunterDPS].Value;
        ValidEnemies = _enemyPools.Where(i => !_enemySources[i.EnemyName].CanAttack && _enemySources[i.EnemyName].HP / hunterDPS < 10).ToList();
    }
}