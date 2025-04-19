using System;
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal interface ISwDynamicConfigManager : ISwConfigAccessor
    {
        event Action<Dictionary<string, object>, ESwConfigSource> OnConfigUpdated; 
        void AddProvider(ISwDynamicConfigProvider provider);
        List<string> GetKeysBySource(ESwConfigSource source);
    }
}