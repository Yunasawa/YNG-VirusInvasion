using System;
using YNL.Bases;

public static partial class Construct
{
    public static Action<FarmConstruct> OnFarmGenerateResource { get; set; }
    public static Action<FarmConstruct, int> OnFarmCountdown { get; set; }
}