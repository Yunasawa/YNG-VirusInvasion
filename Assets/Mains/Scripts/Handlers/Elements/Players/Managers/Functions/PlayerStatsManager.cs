using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;

public class PlayerStatsManager : MonoBehaviour
{
    private PlayerStats _playerStats => Game.Data.PlayerStats;
    private CapacityStats _capacityStats => Game.Data.CapacityStats;

    private void Awake()
    {
        Player.OnCollectEnemyDrops += OnCollectEnemyDrops;
        Player.OnExchangeResources += OnExchangeResources;
        Player.OnExtraStatsLevelUp += OnExtraStatsLevelUp;
        Player.OnFarmStatsLevelUp += OnFarmStatsLevelUp;
        Player.OnCollectFarmResources += OnCollectFarmResources;
        Player.OnConsumeResources += OnConsumeResources;
        Player.OnEnterHomeBase += OnEnterHomeBase;
    }

    private void OnDestroy()
    {
        Player.OnCollectEnemyDrops -= OnCollectEnemyDrops;
        Player.OnExchangeResources -= OnExchangeResources;
        Player.OnExtraStatsLevelUp -= OnExtraStatsLevelUp;
        Player.OnFarmStatsLevelUp -= OnFarmStatsLevelUp;
        Player.OnCollectFarmResources -= OnCollectFarmResources;
        Player.OnConsumeResources -= OnConsumeResources;
        Player.OnEnterHomeBase -= OnEnterHomeBase;
    }

    private void Start()
    {
        for (byte i = 0; i < 6; i++) Game.Data.PlayerStats.UpdateAttributes((AttributeType)i);
    }



    public void OnCollectEnemyDrops((ResourceType type, uint amount)[] drops)
    {
        foreach (var drop in drops)
        {
            _capacityStats.AdjustResources(drop.type, (int)drop.amount);
        }
        Player.OnChangeCapacity?.Invoke();
    }

    private void OnExtraStatsLevelUp(string key)
    {
        _playerStats.ExtraStatsLevel[key].Level++;
        Player.OnExtraStatsUpdate?.Invoke(key);
    }

    private void OnFarmStatsLevelUp(string key)
    {
        _playerStats.FarmStats[Construct.CurrentConstruct][key].Level++;
        Player.OnFarmStatsUpdate?.Invoke(key);
    }

    private void OnExchangeResources(ResourcesInfo from, ResourcesInfo to)
    {
        Game.Data.PlayerStats.Resources[from.Type] -= from.Amount;
        Game.Data.PlayerStats.Resources[to.Type] += to.Amount;
        Player.OnChangeResources?.Invoke();
    }

    private void OnCollectFarmResources(ResourceType type, float amount)
    {
        Game.Data.PlayerStats.Resources[type] += amount;
        Player.OnChangeResources?.Invoke();
    }

    private void OnConsumeResources(ResourceType type, float amount)
    {
        Game.Data.PlayerStats.Resources[type] -= amount;
        Player.OnChangeResources?.Invoke();
    }

    private void OnEnterHomeBase()
    {
        foreach (var resource in Game.Data.CapacityStats.Resources)
        {
            if (resource.Value > 0)
            {
                Game.Data.PlayerStats.AdjustResources(resource.Key, (int)resource.Value);
            }
        }
        Game.Data.CapacityStats.ClearCapacity();
        Player.OnChangeResources?.Invoke();
        Player.OnChangeCapacity?.Invoke();
    }
}