using System.Collections.Generic;
using System.Linq;

namespace SupersonicWisdomSDK
{
    internal abstract class SwDynamicConfigManagerBase : ISwDynamicConfigManager
    {
        #region --- Fields ---
        
        private readonly Dictionary<string, (object Value, ESwConfigSource Source, int Priority)> _dynamicConfig = new();
        private readonly Dictionary<ESwConfigSource, List<string>> _sourceKeyMapping = new();
        
        #endregion
        
        
        #region --- Events ---
        
        public event System.Action<Dictionary<string, object>, ESwConfigSource> OnConfigUpdated;
        
        #endregion
        
        
        #region --- Public Methods ---

        public void AddProvider(ISwDynamicConfigProvider provider)
        {
            if (provider == null) return;
            
            var config = provider.GetConfig();
            MergeConfig(config, provider.Source, (int)provider.Source);
            provider.OnConfigUpdated = updatedConfig => OnProviderConfigUpdated(updatedConfig, provider.Source, (int)provider.Source);
        }

        public Dictionary<string, object> AsDictionary()
        {
            return _dynamicConfig.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Value);
        }

        public int GetValue(string key, int defaultVal)
        {
            return GetValue(key, defaultVal, out _);
        }

        public int GetValue(int defaultVal, params string[] keys)
        {
            return SwUtils.Parsing.GetFirstNonDefault(defaultVal, key => GetValue(key, defaultVal), keys);
        }

        public bool GetValue(string key, bool defaultVal)
        {
            return GetValue(key, defaultVal, out _);
        }

        public bool GetValue(bool defaultVal, params string[] keys)
        {
            return SwUtils.Parsing.GetFirstNonDefault(defaultVal, key => GetValue(key, defaultVal), keys);
        }

        public float GetValue(string key, float defaultVal)
        {
            return GetValue(key, defaultVal, out _);
        }

        public string GetValue(string key, string defaultVal)
        {
            return GetValue(key, defaultVal, out _);
        }

        public bool HasConfigKey(string key)
        {
            return _dynamicConfig.ContainsKey(key);
        }

        public List<string> GetKeysBySource(ESwConfigSource source)
        {
            return _sourceKeyMapping.TryGetValue(source, out var value) ? value : new List<string>();
        }
        
        #endregion
        
        
        #region --- Private Methods ---
        
        protected void LoadConfig(Dictionary<ESwConfigSource, Dictionary<string, object>> configSources)
        {
            foreach (var source in configSources)
            {
                MergeConfig(source.Value, source.Key, (int)source.Key);
            }
        }

        protected void OnProviderConfigUpdated(Dictionary<string, object> updatedConfig, ESwConfigSource source, int priority)
        {
            MergeConfig(updatedConfig, source, priority);
        }

        protected void MergeConfig(Dictionary<string, object> newConfig, ESwConfigSource source, int priority)
        {
            if (newConfig == null) return;
            
            foreach (var kvp in newConfig)
            {
                if (_dynamicConfig.ContainsKey(kvp.Key) && _dynamicConfig[kvp.Key].Priority < priority) continue;
                
                _dynamicConfig[kvp.Key] = (kvp.Value, source, priority);
                
                if (!_sourceKeyMapping.ContainsKey(source))
                {
                    _sourceKeyMapping[source] = new List<string>();
                }
                
                if (!_sourceKeyMapping[source].Contains(kvp.Key))
                {
                    _sourceKeyMapping[source].Add(kvp.Key);
                }
            }
            
            OnConfigUpdated?.Invoke(AsDictionary(), source);
        }

        private float GetValue(string key, float defaultVal, out ESwConfigSource source)
        {
            source = 0;
            var returnValue = defaultVal;
            var keyExist = _dynamicConfig.TryGetValue(key, out var val);

            if (keyExist)
            {
                if (!float.TryParse(val.Value.ToString(), out var floatVal))
                {
                    return returnValue;
                }

                source = val.Source;
                returnValue = floatVal;
            }
            
            LogValueAccess(key, returnValue, source, !keyExist);

            return returnValue;
        }

        private int GetValue(string key, int defaultVal, out ESwConfigSource source)
        {
            source = 0;
            var returnValue = defaultVal;
            var keyExist = _dynamicConfig.TryGetValue(key, out var val);

            if (keyExist)
            {
                if (!int.TryParse(val.Value.ToString(), out var intVal))
                {
                    return returnValue;
                }

                source = val.Source;
                returnValue = intVal;
            }

            LogValueAccess(key, returnValue, source, !keyExist);

            return returnValue;
        }

        private bool GetValue(string key, bool defaultVal, out ESwConfigSource source)
        {
            source = 0;
            var returnValue = defaultVal;
            var keyExist = _dynamicConfig.TryGetValue(key, out var val);

            if (keyExist)
            {
                if (!bool.TryParse(val.Value.ToString(), out var boolVal))
                {
                    return returnValue;
                }

                source = val.Source;
                returnValue = boolVal;
            }
            
            LogValueAccess(key, returnValue, source, !keyExist);

            return returnValue;
        }

        private string GetValue(string key, string defaultVal, out ESwConfigSource source)
        {
            source = 0;
            var returnValue = defaultVal;
            var keyExist = _dynamicConfig.TryGetValue(key, out var val);

            if (keyExist)
            {
                source = val.Source;
                returnValue = val.Value?.ToString();
            }
            
            LogValueAccess(key, returnValue, source, !keyExist);

            return returnValue;
        }

        private static void LogValueAccess<T>(string key, T value, ESwConfigSource source, bool isDefault)
        {
            var valueType = isDefault ? "Default" : "Configured";
            SwInfra.Logger.Log(EWisdomLogType.Config, $"[{valueType} Value] Key: {key}, Value: {value}, Source: {source}");
        }
        
        #endregion
    }
}
