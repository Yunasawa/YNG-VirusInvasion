#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal abstract class SwBaseExternalParamsManager
    {
        #region --- Constants ---
        
        private const float SAVING_INTERVAL = 3f;

        #endregion
        

        #region --- Members ---

        private bool _didChanged;
        private Coroutine _coroutine;

        #endregion


        #region --- Properties ---

        protected abstract List<SwExternalParamsData> Config { get; }
        protected abstract string StoreKey { get; }
        protected abstract EWisdomLogType LogType { get; }

        protected Dictionary<string, object> KeyToValueDict { get; private set; }
        private Dictionary<string, string> NameToKeyDict { get; set; }

        #endregion


        #region --- Construction ---

        protected SwBaseExternalParamsManager()
        {
            SwInfra.Logger.Log(LogType, "Start");
            
            var config = Config;
            
            SwInfra.Logger.Log(LogType, $"Is Config empty - {config.SwIsNullOrEmpty()}");

            if (!config.SwIsNullOrEmpty())
            {
                Init(Config);
            }
        }

        #endregion


        #region --- Private Methods ---

        protected virtual void Init(List<SwExternalParamsData> config)
        {
            SwInfra.Logger.Log(LogType, $"Init - {config.Count} items");
            
            var oldDict = SwInfra.KeyValueStore.GetGenericSerializedData(StoreKey, new Dictionary<string, object>());
            KeyToValueDict = SwExternalParamsUtils.InitAndLoadKeyToValueDict(config, LogType);
            KeyToValueDict.SwReplaceExistingKeys(oldDict);

            NameToKeyDict = new Dictionary<string, string>();

            foreach (var configurationData in config)
            {
                NameToKeyDict[configurationData.name] = configurationData.key;
            }
            
            SwInfra.Logger.Log(LogType, $"KeyToValueDict = {SwUtils.JsonHandler.SerializeObject(KeyToValueDict)}");
            SwInfra.Logger.Log(LogType, $"NameToKeyDict = {SwUtils.JsonHandler.SerializeObject(NameToKeyDict)}");
        }

        protected void SetDataByName<T>(string name, T value)
        {
            SetData(NameToKeyDict.SwSafelyGet(name, string.Empty), value);
        }

        private void SetData<T>(string key, T value)
        {
            if (!KeyToValueDict.ReplaceValueForSpecificValueType(key, value, LogType)) return;

            SwInfra.Logger.Log(LogType, $"Did changed {key} to {value?.ToString() ?? "null"}");
            _didChanged = true;
            _coroutine ??= SwInfra.CoroutineService.RunActionEndlessly(TrySaveState, SAVING_INTERVAL, () => false);
        }

        private void TrySaveState()
        {
            if (!_didChanged) return;
            
            _didChanged = false;
            SwInfra.KeyValueStore.SetGenericSerializedData(StoreKey, KeyToValueDict);
        }

        #endregion
    }
}
#endif