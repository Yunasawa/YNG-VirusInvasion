using System;
using YNL.Bases;

public static partial class Player
{
    public static Action<(ResourceType, uint)[]> OnCollectEnemyDrops { get; set; }
    public static Action<bool, ConstructType, ConstructManager> OnInteractWithConstruct { get; set; }
    public static Action<string> OnExtraStatsUpdate { get; set; }
    public static Action<ResourcesInfo, ResourcesInfo> OnExchangeResources { get; set; }
}