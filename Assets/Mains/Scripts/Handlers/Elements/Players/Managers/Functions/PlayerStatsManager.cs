using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

public class PlayerStatsManager : MonoBehaviour
{
    private PlayerStats _playerStats => Game.Data.PlayerStats;
    private CapacityStats _capacityStats => Game.Data.CapacityStats;
    private RuntimeConstructStats _runtimeConstructStats => Game.Data.RuntimeStats.ConstructStats;

    [SerializeField] private HealthFieldUI _healthField;

    private int _totalDamage = 0;

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

        HandleHealth().Forget();
    }

    private void OnChangeStats()
    {
        for (byte i = 0; i < 6; i++) _playerStats.UpdateAttributes((AttributeType)i);
    }

    public void OnCollectEnemyDrops(int capacity, SerializableDictionary<ResourceType, uint> drops)
    {
        _capacityStats.CollectDrops(capacity, drops);
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

    private void OnExchangeResources(string name, ResourcesInfo from, ResourcesInfo to)
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

        if (Game.Data.PlayerStats.Resources[ResourceType.Food1] >= 23)
        {
            Game.Input.TutorialPanelUI.UpgradeAttributeTutorial.ShowTutorial();
        }
    }

    public void TakeDamage(int damage)
    {
        _playerStats.CurrentHP -= damage;
        if (_playerStats.CurrentHP <= 0)
        {

        }

        _healthField.UpdateHealth();
    }

    private async UniTaskVoid HandleHealth()
    {
        while (true)
        {
            await UniTask.WaitForSeconds(1);

            Damage();
            Heal();
        }

        void Heal()
        {
            if (_playerStats.CurrentHP >= _playerStats.MaxHP) return;

            _playerStats.CurrentHP += Formula.Stats.GetHeal();
            if (_playerStats.CurrentHP > _playerStats.MaxHP) _playerStats.CurrentHP = _playerStats.MaxHP;

            _healthField.UpdateHealth();
        }

        void Damage()
        {
            _totalDamage = 0;

            foreach (var group in Player.Enemy.Tentacles)
            {
                if (group.Enemy.IsNull()) continue;
                _totalDamage += Game.Data.EnemySources[group.Enemy.Stats.ID].AttackDamage;
            }

            TakeDamage(_totalDamage);
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