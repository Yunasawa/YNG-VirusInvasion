using System;
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwAbInternalHandler : ISwDynamicConfigProvider
    {
        #region --- Constants ---

        internal const string AB_REMAINING_DAYS = "abRemainingDays";
        private const string AB_CONCLUDED_EVENT_NAME = "AbConcluded";
        private const string AB_CONCLUSION_SCOPE = "abConclusionScope";

        internal const string SW_INTERNAL_AB_CONCLUDED = "swInternalAbConcluded";
        internal const string SW_EXTERNAL_AB_CONCLUDED = "swExtrnalAbConcluded";
        private const string INTERNAL_AB = "Internal";
        private const string EXTERNAL_AB = "External";
        
        internal const string AB_ID = "abId";
        internal const string AB_NAME = "abName";
        internal const string AB_VARIANT = "abVariant";

        #endregion

        
        #region --- Fields ---
        
        private readonly SwCoreTracker _tracker;
        private readonly SwCoreUserData _coreUserData;
        
        private ISwConfigManagerState _configManagerState;
        private SwAbConfig _ab;
        private bool _internalAbConcluded;
        private bool _externalAbConcluded;

        #endregion

        
        #region --- Events ---
        
        internal event Action OnAbUpdated;

        #endregion

        
        #region --- Properties ---
        
        public ESwConfigSource Source { get; } = ESwConfigSource.WisdomAb;
        public Action<Dictionary<string, object>> OnConfigUpdated { get; set; }
        
        internal SwAbConfig Ab
        {
            get { return _ab; }
            private set { _ab = value ?? _ab; }
        }
        
        private bool CanConcludeInternalAb
        {
            get { return !_internalAbConcluded && _configManagerState?.Timing <= EConfigListenerType.GameStarted; }
        }

        private bool CanConcludeExternalAb
        {
            get
            {
                return !_externalAbConcluded && _configManagerState?.Timing <= EConfigListenerType.FinishWaitingForRemote;
            }
        }

        #endregion

        
        #region --- Constructor ---
        
        public SwAbInternalHandler(SwCoreUserData userData, SwCoreTracker tracker)
        {
            _tracker = tracker;
            _coreUserData = userData;
        }

        #endregion

        
        #region --- Public Methods ---
        
        public Dictionary<string, object> GetConfig()
        {
            if (_ab == null)
            {
                return new Dictionary<string, object>();
            }
            
            var abConfig = SwConfigUtils.ResolveAbConfig(_ab);
            
            return abConfig;
        }
        
        internal void InitHandler(ISwConfigManagerState configManagerState)
        {
            _configManagerState = configManagerState;
            var config = DeserializeInitialAbConfig();
            ResolveIsAbConcluded();
            UpdateAbInternal(config);
        }
        
        internal bool ShouldSaveAb(HashSet<string> predefinedKeys, string abKey)
        {
            SwInfra.Logger.Log(EWisdomLogType.Config, $"{nameof(_internalAbConcluded)} = {_internalAbConcluded} | {nameof(_externalAbConcluded)} = {_externalAbConcluded} | {nameof(abKey)} = {abKey}");

            if (_externalAbConcluded && _internalAbConcluded)
            {
                return false;
            }
            
            if (string.IsNullOrEmpty(abKey))
            {
                return true;
            }
            
            var isInternalAb = predefinedKeys.Contains(abKey);
            
            return isInternalAb ? CanConcludeInternalAb : CanConcludeExternalAb;
        }

        internal void TryTrackInternalAbConcludedEvent()
        {
            if (_internalAbConcluded) return;

            _internalAbConcluded = true;
            SwInfra.KeyValueStore.SetBoolean(SW_INTERNAL_AB_CONCLUDED, _internalAbConcluded);
            SendAbConcludedEvent(INTERNAL_AB);
        }

        internal void TryTrackExternalAbConcludedEvent()
        {
            if (_externalAbConcluded) return;

            _externalAbConcluded = true;
            SwInfra.KeyValueStore.SetBoolean(SW_EXTERNAL_AB_CONCLUDED, _externalAbConcluded);
            SendAbConcludedEvent(EXTERNAL_AB);
        }

        internal void UpdateAb(ref SwCoreConfig config, SwAbConfig abConfig)
        {
            Ab = abConfig ?? Ab ?? new SwAbConfig();

            if (config == null)
            {
                return;
            }
            
            config.Ab = Ab;
            
            if (config.Ab is { IsValid: true, IsActive: true })
            {
                UpdateAbInternal(config.Ab);
            }

            SwInfra.Logger.Log(EWisdomLogType.Config, $"Config.ab: {config.Ab}");
            AfterAbUpdate();
        }
        
        #endregion

        
        #region --- Private Methods ---

        private void UpdateAbInternal(SwAbConfig abConfig)
        {
            if (abConfig == null) return;
            
            var config = SwConfigUtils.ResolveAbConfig(abConfig);
            Ab = abConfig;
            OnConfigUpdated?.Invoke(config); 
            AfterAbUpdate();
        }
        
        private void AfterAbUpdate()
        {
            OnAbUpdated?.Invoke();
        }
        
        private void ResolveIsAbConcluded()
        {
            if (_coreUserData.IsNew)
            {
                SetAbConcludedFlags(false, false);
            }
            else
            {
                var comeFromUpdate = !SwInfra.KeyValueStore.HasKey(SW_EXTERNAL_AB_CONCLUDED) || !SwInfra.KeyValueStore.HasKey(SW_INTERNAL_AB_CONCLUDED);
                
                if (comeFromUpdate)
                {
                    SetAbConcludedFlags(true, true);
                }
                else
                {
                    _internalAbConcluded = SwInfra.KeyValueStore.GetBoolean(SW_INTERNAL_AB_CONCLUDED);
                    _externalAbConcluded = SwInfra.KeyValueStore.GetBoolean(SW_EXTERNAL_AB_CONCLUDED);
                }
            }
        }
        
        private void SetAbConcludedFlags(bool internalFlag, bool externalFlag)
        {
            _internalAbConcluded = internalFlag;
            _externalAbConcluded = externalFlag;

            SwInfra.KeyValueStore.SetBoolean(SW_INTERNAL_AB_CONCLUDED, _internalAbConcluded);
            SwInfra.KeyValueStore.SetBoolean(SW_EXTERNAL_AB_CONCLUDED, _externalAbConcluded);
        }

        private void SendAbConcludedEvent(string abConclusionScope)
        {
            var customs = SwCoreTracker.GenerateEventCustoms(AB_CONCLUDED_EVENT_NAME, abConclusionScope);
            customs.SwAddOrReplace(AB_CONCLUSION_SCOPE, abConclusionScope);

            var remainingDays = Ab?.RemainingDays; // The A/B object still could be null here.
            
            if (remainingDays is > 0)
            {
                // This A/B Test has termination conditions
                customs.SwAddOrReplace(AB_REMAINING_DAYS, remainingDays.Value);
            }
            
            _tracker?.TrackInfraEvent(customs);
        }

        private SwAbConfig DeserializeInitialAbConfig()
        {
            var abConfigString = SwInfra.KeyValueStore.GetString(SwStoreKeys.InitialConfig);
            
            if (abConfigString.SwIsNullOrEmpty())
            {
                return null;
            }

            try
            {
                return SwUtils.JsonHandler.DeserializeObject<SwCoreConfig>(abConfigString).Ab;
            }
            catch (Exception ex)
            {
                SwInfra.Logger.LogException(ex, EWisdomLogType.Config, $"An error occurred while trying to parse initial A/B config");
            }
            
            return null;
        }
        
        #endregion
    }
}