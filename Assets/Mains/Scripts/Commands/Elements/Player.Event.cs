using System;
using YNL.Bases;

public static partial class Player
{
    public static Action<(ResourceType, uint)[]> OnCollectEnemyDrops { get; set; }
    public static Action<bool, ConstructType> OnInteractWithConstruct { get; set; }
    public static Action<string> OnExtraStatsUpdate { get; set; }
}