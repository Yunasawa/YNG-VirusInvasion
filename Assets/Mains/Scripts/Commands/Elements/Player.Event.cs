using System;
using YNL.Bases;

public static partial class Player
{
    public static Action<(ResourceType, uint)[]> OnCollectEnemyDrops { get; set; }
    public static Action<bool, ConstructType, ConstructManager> OnInteractWithConstruct { get; set; }
    public static Action<ResourcesInfo, ResourcesInfo> OnExchangeResources { get; set; }
    public static Action<ResourceType, int> OnCollectFarmResources { get; set; }
    public static Action<ResourceType, int> OnConsumeResources { get; set; }
    public static Action OnChangeResources { get; set; }

    public static Action<string> OnExtraStatsUpdate { get; set; }
    public static Action<string> OnFarmStatsUpdate { get; set; }

    public static Action<string> OnExtraStatsLevelUp { get; set; }
    public static Action<string> OnFarmStatsLevelUp { get; set; }

    public static Action<StageType, StageType> OnEnterStage { get; set; }
}