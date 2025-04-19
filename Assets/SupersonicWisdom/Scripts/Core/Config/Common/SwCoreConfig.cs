using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwCoreConfig : ISwCoreInternalConfig
    {
        #region --- Constants ---

        private const string CONFIG_KEY = "config";

        #endregion
        

        #region --- Properties ---
        
        /// <summary>
        ///     The dictionary under "config" key.
        ///     This cannot be simply deserialized since it's a dynamic dictionary
        /// </summary>
        [JsonProperty("config")]
        [JsonConverter(typeof(DictionaryConverter<object>))]
        public Dictionary<string, object> DynamicConfig { get; private set; }

        [JsonProperty("ab")]
        public SwAbConfig Ab { get; set; }
        
        [JsonProperty("tac")]
        public SwTacConfig Tac { get; set; }

        #endregion


        #region --- Construction ---

        public SwCoreConfig(Dictionary<string, object> defaultDynamicConfig)
        {
            DynamicConfig = defaultDynamicConfig;
        }

        #endregion


        #region --- Public Methods ---

        public Dictionary<string, object> AsDictionary()
        {
            return DynamicConfig.AsDictionary();
        }

        #endregion
        
        
        #region --- Private Methods ---

        protected T ParseKeyFromDynamicConfigToObject<T>(string triggerDataStringify) where T : new()
        {
            if (!triggerDataStringify.SwIsNullOrEmpty())
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(triggerDataStringify);
                }
                catch (Exception e)
                {
                    SwInfra.Logger.LogError(EWisdomLogType.TAC, e);
                }
            }

            return new T();
        }

        protected internal virtual void ResolveDynamicConfig(Dictionary<string, object> dict) { }

        #endregion
    }
}