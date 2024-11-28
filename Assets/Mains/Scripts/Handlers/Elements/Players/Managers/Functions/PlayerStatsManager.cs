using UnityEngine;
using YNL.Bases;

public class PlayerStatsManager : MonoBehaviour
{
    private PlayerStats _stats => Game.Data.PlayerStats;

    private void Awake()
    {
        Player.OnCollectEnemyDrops += OnCollectEnemyDrops;
        Player.OnExchangeResources += OnExchangeResources;
        Player.OnExtraStatsLevelUp += OnExtraStatsLevelUp;
        Player.OnFarmStatsLevelUp += OnFarmStatsLevelUp;
    }

    private void OnDestroy()
    {
        Player.OnCollectEnemyDrops -= OnCollectEnemyDrops;
        Player.OnExchangeResources -= OnExchangeResources;
        Player.OnExtraStatsLevelUp -= OnExtraStatsLevelUp;
        Player.OnFarmStatsLevelUp -= OnFarmStatsLevelUp;
    }

    private void Start()
    {
        for (byte i = 0; i < 6; i++) UpdateAttributeValues((AttributeType)i);
    }

    public void UpdateAttributeValues(AttributeType type)
    {
        switch (type)
        {
            case AttributeType.DPS: Game.Data.PlayerStats.DPS = Formula.Stats.GetDPS(); break;
            case AttributeType.MS: Game.Data.PlayerStats.MS = Formula.Stats.GetMS(); break;
            case AttributeType.Capacity: Game.Data.PlayerStats.Capacity = Formula.Stats.GetCapacity(); break;
            case AttributeType.Radius: Game.Data.PlayerStats.Radius = Formula.Stats.GetRadius(); break;
            case AttributeType.Tentacle: Game.Data.PlayerStats.Tentacle = Formula.Stats.GetTentacle(); break;
            case AttributeType.HP: Game.Data.PlayerStats.HP = Formula.Stats.GetHP(); break;
        }
    }

    public void OnCollectEnemyDrops((ResourceType type, uint amount)[] drops)
    {
        foreach (var drop in drops) _stats.AdjustResources(drop.type, (int)drop.amount);
    }

    private void OnExtraStatsLevelUp(string key)
    {
        _stats.ExtraStatsLevel[key].Level++;
        Player.OnExtraStatsUpdate?.Invoke(key);
    }

    private void OnFarmStatsLevelUp(string key)
    {
        _stats.FarmStats[Construct.CurrentConstruct][key].Level++;
        Player.OnFarmStatsUpdate?.Invoke(key);
    }

    private void OnExchangeResources(ResourcesInfo from, ResourcesInfo to)
    {
        Game.Data.PlayerStats.Resources[from.Type] -= from.Amount;
        Game.Data.PlayerStats.Resources[to.Type] += to.Amount;
        View.OnUpdateResourceNodes?.Invoke();
    }
}