using System;
using YNL.Bases;

public static partial class Player
{
    public static Action<(ResourceType, uint)[]> OnCollectEnemyDrops { get; set; }
}