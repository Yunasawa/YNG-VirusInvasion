using System;
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwLocalConfigHandler : ISwDynamicConfigProvider
    {
        #region --- Members ---

        private readonly Dictionary<string, object> _configValues;

        #endregion
        
        
        #region --- Constructor ---
        
        public SwLocalConfigHandler(ISwLocalConfigProvider[] localConfigProviders)
        {
            _configValues = ResolveLocalConfigFromProviders(localConfigProviders);
        }
        
        #endregion
        
        
        #region --- Properties ---

        public ESwConfigSource Source { get; } = ESwConfigSource.WisdomLocal;
        public Action<Dictionary<string, object>> OnConfigUpdated { get; set; }
        
        #endregion
        
        
        #region --- Public Methods ---
        
        public Dictionary<string, object> GetConfig()
        {
            return _configValues;
        }
        
        #endregion


        #region --- Private Methods ---

        private Dictionary<string, object> ResolveLocalConfigFromProviders(ISwLocalConfigProvider[] configProviders)
        {
            var arrayLength = configProviders.Length;

            if (arrayLength == 0) return new Dictionary<string, object>();

            var localConfigValues = new Dictionary<string, object>[arrayLength];

            for (var i = 0; i < arrayLength; i++)
            {
                var config = configProviders[i].GetLocalConfig();
                localConfigValues[i] = config.LocalConfigValues;
            }

            var configToMerge = new Dictionary<string, object>().SwMerge(true, localConfigValues);
            SwInfra.Logger.Log(EWisdomLogType.Config, "SwLocalConfigHandler | ResolveLocalConfigFromSetters | " + $"Resolved {configToMerge.Count} pairs");

            return configToMerge;
        }

        #endregion
    }
}