using System;
using YNL.Bases;
using YNL.Utilities.Addons;

public static partial class Player
{
    public static Action OnLevelUp { get; set; }
    public static Action<int, SerializableDictionary<ResourceType, uint>> OnCollectEnemyDrops { get; set; }
    public static Action<int> OnCollectEnemyExp { get; set; }
    public static Action<bool, ConstructType, ConstructManager> OnInteractWithConstruct { get; set; }
    public static Action<string, ResourcesInfo, ResourcesInfo> OnExchangeResources { get; set; }
    public static Action<ResourceType, float> OnCollectFarmResources { get; set; }
    public static Action<ResourceType, float> OnConsumeResources { get; set; }
    public static Action OnChangeResources { get; set; }
    public static Action OnChangeCapacity { get; set; }
    public static Action OnChangeStats { get; set; }
    public static Action<(ResourceType, int)[]> OnReturnCapacity { get; set; }
    public static Action<AttributeType> OnUpgradeAttribute { get; set; }
    public static Action<bool, int> OnChangeHP { get; set; }
    public static Action OnUpgradeRevolution { get; set; }

    public static Action<string> OnExtraStatsUpdate { get; set; }
    public static Action<string> OnFarmStatsUpdate { get; set; }
    public static Action<string> OnExtraStatsLevelUp { get; set; }
    public static Action<string> OnFarmStatsLevelUp { get; set; }
    public static Action<string> OnTradeInMarket { get; set; }

    public static Action OnEnterHomeBase { get; set; }
    public static Action<StageType, StageType> OnEnterStage { get; set; }

    public static Action<string, QuestBillboardUI> OnOpenQuestWindow { get; set; }

    public static Action<string> OnDefeatEnemy { get; set; }
    public static Action OnDeath { get; set; }
    public static Action OnRespawn { get; set; }
}