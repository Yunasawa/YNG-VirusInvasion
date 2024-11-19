using System;

public static partial class Construct
{
    public static string CurrentConstruct { get; set; }
    public static Action<string> OnExtraStatsLevelUp { get; set; }
}