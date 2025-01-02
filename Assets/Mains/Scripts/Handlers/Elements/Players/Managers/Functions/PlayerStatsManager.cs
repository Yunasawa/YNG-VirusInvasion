using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;

public class PlayerStatsManager : MonoBehaviour
{
    private PlayerStats _playerStats => Game.Data.PlayerStats;
    private CapacityStats _capacityStats => Game.Data.CapacityStats;
    private RuntimeConstructStats _runtimeConstructStats => Game.Data.RuntimeStats.ConstructStats;

    [SerializeField] private HealthFieldUI _healthField;

    private void Awake()
    {
        Player.OnCollectEnemyDrops += OnCollectEnemyDrops;
        Player.OnCollectEnemyExp += OnCollectEnemyExp;
        Player.OnExchangeResources += OnExchangeResources;
        Player.OnExtraStatsLevelUp += OnExtraStatsLevelUp;
        Player.OnFarmStatsLevelUp += OnFarmStatsLevelUp;
        Player.OnCollectFarmResources += OnCollectFarmResources;
        Player.OnConsumeResources += OnConsumeResources;
        Player.OnEnterHomeBase += OnEnterHomeBase;
        Player.OnChangeStats += OnChangeStats;
        Player.OnUpgradeAttribute += OnUpgradeAttribute;
    }

    private void OnDestroy()
    {
        Player.OnCollectEnemyDrops -= OnCollectEnemyDrops;
        Player.OnCollectEnemyExp -= OnCollectEnemyExp;
        Player.OnExchangeResources -= OnExchangeResources;
        Player.OnExtraStatsLevelUp -= OnExtraStatsLevelUp;
        Player.OnFarmStatsLevelUp -= OnFarmStatsLevelUp;
        Player.OnCollectFarmResources -= OnCollectFarmResources;
        Player.OnConsumeResources -= OnConsumeResources;
        Player.OnEnterHomeBase -= OnEnterHomeBase;
        Player.OnChangeStats -= OnChangeStats;
        Player.OnUpgradeAttribute -= OnUpgradeAttribute;
    }

    private void Start()
    {
        OnChangeStats();

        OnUpgradeAttribute(AttributeType.HP);

        ReceiveDamage().Forget();
    }

    private void OnChangeStats()
    {
        for (byte i = 0; i < 6; i++) _playerStats.UpdateAttributes((AttributeType)i);
    }

    public void OnCollectEnemyDrops((ResourceType type, uint amount)[] drops)
    {
        foreach (var drop in drops)
        {
            _capacityStats.AdjustResources(drop.type, Formula.Stats.GetResource(drop.amount));
        }
        Player.OnChangeCapacity?.Invoke();
    }

    public void OnCollectEnemyExp(int exp)
    {
        if (_playerStats.CurrentLevel < 10)
        {
            _playerStats.CurrentExp += exp;
            int maxExp = Formula.Level.GetMaxExp();
            if (_playerStats.CurrentExp >= maxExp)
            {
                int remainExp = _playerStats.CurrentExp - maxExp;
                _playerStats.CurrentLevel = Mathf.Min(10, _playerStats.CurrentLevel + 1);
                _playerStats.CurrentExp = remainExp;

                Player.OnLevelUp?.Invoke();
            }

            View.OnChangeLevelField?.Invoke();
        }
    }

    private void OnExtraStatsLevelUp(string key)
    {
        _playerStats.ExtraStatsLevel[key].Level++;
        Player.OnExtraStatsUpdate?.Invoke(key);
    }

    private void OnFarmStatsLevelUp(string key)
    {
        _runtimeConstructStats.Farms[Construct.CurrentConstruct].Attributes[key].Level++;
        Player.OnFarmStatsUpdate?.Invoke(key);
    }

    private void OnExchangeResources(ResourcesInfo from, ResourcesInfo to)
    {
        _playerStats.Resources[from.Type] -= from.Amount;
        _playerStats.Resources[to.Type] += to.Amount;
        Player.OnChangeResources?.Invoke();
    }

    private void OnCollectFarmResources(ResourceType type, float amount)
    {
        _playerStats.Resources[type] += amount;
        Player.OnChangeResources?.Invoke();
    }

    private void OnConsumeResources(ResourceType type, float amount)
    {
        _playerStats.Resources[type] -= amount;
        Player.OnChangeResources?.Invoke();
    }

    private void OnEnterHomeBase()
    {
        Dictionary<ResourceType, int> resources = new();

        foreach (var resource in Game.Data.CapacityStats.Resources)
        {
            if (resource.Value > 0)
            {
                _playerStats.AdjustResources(resource.Key, (int)resource.Value);

                if (!resources.ContainsKey(resource.Key)) resources.Add(resource.Key, (int)resource.Value);
                else resources[resource.Key] += (int)resource.Value;
            }
        }
        Game.Data.CapacityStats.Reset();
        Player.OnChangeResources?.Invoke();
        Player.OnChangeCapacity?.Invoke();

        (ResourceType type, int amount)[] capacity = resources.Select(kv => (type: kv.Key, amount: kv.Value)).ToArray();
        Player.OnReturnCapacity?.Invoke(capacity);
    }

    public void TakeDamage(int damage)
    {
        _playerStats.CurrentHP -= damage;
        if (_playerStats.CurrentHP <= 0)
        {

        }

        _healthField.UpdateHealth();
    }

    private async UniTaskVoid ReceiveDamage()
    {
        while (true)
        {
            await UniTask.WaitForSeconds(1);

            int totalDamage = 0;

            foreach (var enemy in Player.Enemy.Enemies)
            {
                totalDamage += Game.Data.EnemySources[enemy.Stats.ID].AttackDamage;
            }

            TakeDamage(totalDamage);
        }
    }

    private void OnUpgradeAttribute(AttributeType type)
    {
        if (type == AttributeType.HP)
        {
            _playerStats.MaxHP = Formula.Stats.GetHP();
            _healthField.UpdateHealth();
        }
    }
}