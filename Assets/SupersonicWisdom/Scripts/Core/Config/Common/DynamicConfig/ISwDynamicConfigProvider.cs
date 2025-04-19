using System;
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal interface ISwDynamicConfigProvider
    {
        ESwConfigSource Source { get; }
        Dictionary<string, object> GetConfig();
        Action<Dictionary<string, object>> OnConfigUpdated { get; set; }
    }
}