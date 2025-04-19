using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal abstract class SwCoreConfigManager : ISwConfigManager, ISwReadyEventListener, ISwGameStateSystemListener, ISwTrackerDataProvider
    {
        #region --- Constants ---

        internal const int MAXIMUM_REQUEST_WAITING_TIME = 2;
        private const string HOST_APP_ERROR_ON_CALLBACK = "HostAppErrorOnCallback";
        
        #endregion


        #region --- Events ---

        private event OnLoaded LoadedEvent;

        #endregion


        #region --- Members ---

        private readonly SwCoreTracker _tracker;
        protected SwCoreNativeAdapter _nativeAdapter;

        private bool _didInit;
        private bool _configArrived;
        private SwCoreConfig _coreConfig;
        private HashSet<string> _predefinedKeys;
        
        private readonly SwWisdomRemoteConfigHandler _wisdomRemoteConfigHandler;
        private readonly SwAbInternalHandler _abInternalHandler;
        private readonly List<ISwCoreConfigListener> _listeners;
        private readonly ISwDynamicConfigManager _dynamicConfigManager;

        #endregion


        #region --- Properties ---

        public bool DidResolve { get; private set; }
        public bool IsTimeout { get; private set; }
        public EConfigStatus Status { get; private set; }

        public SwCoreConfig Config
        {
            get { return _coreConfig;}
            private set
            {
                _coreConfig = value;
            }
        }
        
        public ISwDynamicConfigManager DynamicConfig
        {
            get { return _dynamicConfigManager; }
        }
        
        public SwWebRequestError WebRequestError { get; private set; }
        public EConfigListenerType Timing { get; set; }

        #endregion


        #region --- Construction ---

        protected SwCoreConfigManager(ISwDynamicConfigManager dynamicConfigManager, SwCoreTracker tracker, SwCoreNativeAdapter nativeAdapter, SwWisdomRemoteConfigHandler wisdomRemoteConfigHandler, SwAbInternalHandler abInternalHandler)
        {
            _nativeAdapter = nativeAdapter;
            _tracker = tracker;
            Status = EConfigStatus.NotInitialized;
            _listeners = new List<ISwCoreConfigListener>();
            _wisdomRemoteConfigHandler = wisdomRemoteConfigHandler;
            _abInternalHandler = abInternalHandler;
            _dynamicConfigManager = dynamicConfigManager;
            _coreConfig = new SwCoreConfig(_dynamicConfigManager.AsDictionary());
            _dynamicConfigManager.OnConfigUpdated -= OnDynamicConfigUpdated;
            _dynamicConfigManager.OnConfigUpdated += OnDynamicConfigUpdated;
        }

        #endregion


        #region --- Public Methods ---

        public void Init(ISwLocalConfigProvider[] localConfigProviders)
        {
            Timing = EConfigListenerType.Construction;
            
            InitializeAbHandler();
            InitializeWisdomRemoteConfigHandler();
            LoadLocalConfig(localConfigProviders);
            TryLoadLatestSuccessfulRemoteConfig();
            OnConfigReady(ESwConfigSource.WisdomLocal);
        }
        
        public void OnSwReady()
        {
            Timing = EConfigListenerType.GameStarted;
            _abInternalHandler.TryTrackExternalAbConcludedEvent();
        }

        public void AddListeners(List<ISwCoreConfigListener> listeners)
        {
            _listeners.AddRange(listeners);
        }

        public void AddOnLoadedListener(OnLoaded onLoadedCallback)
        {
            LoadedEvent += onLoadedCallback;
        }
        
        public void OnGameSystemStateChange(SwSystemStateEventArgs eventArgs)
        {
            if (IsNotInGameplayState(eventArgs.NewGameState)) return;

            OnGameStarted();
        }

        public IEnumerator Fetch()
        {
            Timing = EConfigListenerType.FinishWaitingForRemote;
            _didInit = true;
            
            if (!_didInit)
            {
                throw new SwException($"{nameof(SwCoreConfigManager)} | {nameof(Fetch)} | Init method must be called before calling Fetch.");
            }
            
            _wisdomRemoteConfigHandler.FetchRemoteConfig(OnInternalRemoteConfigSuccess);

            yield return new WaitUntil(() => _configArrived || IsTimeout);
        }

        internal void OnTimeout()
        {
            IsTimeout = true;
            WebRequestError = SwWebRequestError.Timeout;
            
            OnConfigReady(ESwConfigSource.WisdomLocal);
        }

        public void RemoveOnLoadedListener(OnLoaded onLoadedCallback)
        {
            LoadedEvent -= onLoadedCallback;
        }
        
        public void OnInternalRemoteConfigSuccess(SwWebResponse response)
        {
            if (_configArrived) return;

            SwInfra.Logger.Log(EWisdomLogType.Config);

            _configArrived = true;

            try
            {
                var parsedRemoteConfig = TryParseRemoteConfig(response);

                if (parsedRemoteConfig == null) return;
                
                if (_abInternalHandler.ShouldSaveAb(_predefinedKeys, parsedRemoteConfig.Ab?.key))
                {
                    SetNewConfig(parsedRemoteConfig, parsedRemoteConfig.Ab, EConfigStatus.Remote);
                    SaveInitialConfigResponse(response.Text);
                }
                else if (parsedRemoteConfig.Ab is { IsInactive: true })
                {
                    SetNewConfig(parsedRemoteConfig, parsedRemoteConfig.Ab, EConfigStatus.Remote);
                }
                else
                {
                    SetNewConfig(parsedRemoteConfig, null, EConfigStatus.Remote);
                }

                SaveCachedConfigResponse(response.Text);

                _abInternalHandler.TryTrackInternalAbConcludedEvent();
                _abInternalHandler.TryTrackExternalAbConcludedEvent();
            }
            finally
            {
                OnConfigReady(ESwConfigSource.WisdomRemote);
                NotifyExternalListeners();
            }
        }
        
        public (SwJsonDictionary dataDictionary, IEnumerable<string> keysToEncrypt) ConditionallyAddExtraDataToTrackEvent(SwCoreUserData coreUserData, string eventName = "")
        {
            var extraData = new SwJsonDictionary
            {
                { SwAbInternalHandler.AB_ID , _abInternalHandler.Ab?.id ?? "" },
                { SwAbInternalHandler.AB_NAME, _abInternalHandler.Ab?.key ?? "" },
                { SwAbInternalHandler.AB_VARIANT, _abInternalHandler.Ab?.group ?? "" },
            };

            return (extraData, KeysToEncrypt());

            IEnumerable<string> KeysToEncrypt()
            {
                yield break;
            }
        }
        
        public abstract SwCoreConfig ParseConfig(string configStr);

        #endregion


        #region --- Private Methods ---
        
        private void OnDynamicConfigUpdated(Dictionary<string, object> updatedConfig, ESwConfigSource source)
        {
            if (updatedConfig == null) return;

            // TODO: refactor and make sure updates from unity gaming services remote config 
            // which can be changed every level/api call should update the features about new config
            // Note: Please also make sure the new dynamic config values will be deserialized into serialized object on-top of the config object
            if (source == ESwConfigSource.UgsRemote)
            {
                OnConfigReady(ESwConfigSource.UgsRemote);
            }
            else
            {
                Config?.ResolveDynamicConfig(updatedConfig);
            }
        }

        private void OnConfigReady(ESwConfigSource source)
        {
            Config?.ResolveDynamicConfig(DynamicConfig.AsDictionary());
            NotifyConfigResolved(source);
        }

        private void NotifyConfigResolved(ESwConfigSource source)
        {
            SwInfra.Logger.Log(EWisdomLogType.Config, $"Config resolved | Timing = {Timing} | Status - {Status} | Source - {source}");

            DidResolve = true;

            NotifyInternalListeners(source);
        }

        private void LoadLocalConfig(ISwLocalConfigProvider[] localConfigProviders = null)
        {
            var localDynamicConfig = new SwLocalConfigHandler(localConfigProviders);
            _dynamicConfigManager.AddProvider(localDynamicConfig);
            _predefinedKeys = localDynamicConfig.GetConfig().Keys.SwToHashSet();
            
            var localConfig = CreateLocalConfig(localDynamicConfig.GetConfig());
            localConfig.ResolveDynamicConfig(localConfig.DynamicConfig);
            SetNewConfig(localConfig, null, EConfigStatus.Local, false);
        }
        
        private void InitializeAbHandler()
        {
            if(_abInternalHandler == null) return;
            
            _abInternalHandler.InitHandler(this);
            _abInternalHandler.OnAbUpdated += OnAbUpdated;
            
            SwInfra.Logger.Log(EWisdomLogType.Config, $"Created and initialized AB manager");
        }
        
        private void InitializeWisdomRemoteConfigHandler()
        {
            if (_wisdomRemoteConfigHandler == null) return;
            
            _wisdomRemoteConfigHandler.InitHandler(this);
            SwInfra.Logger.Log(EWisdomLogType.Config, $"Initialized WisdomRemoteConfigHandler");
        }

        protected abstract SwCoreConfig CreateLocalConfig(Dictionary<string, object> localConfigValues);

        private void NotifyExternalListeners()
        {
            try
            {
                LoadedEvent?.Invoke(Status == EConfigStatus.Remote, WebRequestError);
            }
            catch (Exception e)
            {
                _tracker?.TrackInfraEvent(HOST_APP_ERROR_ON_CALLBACK, e.Message, e.StackTrace);
                SwInfra.Logger.LogException(e, EWisdomLogType.Config, $"{nameof(NotifyInternalListeners)} failed");
            }
        }
        
        private void OnAbUpdated()
        {
            if (_abInternalHandler.Ab == null) return;
            
            _nativeAdapter.UpdateAbData(_abInternalHandler.Ab, Status);
        }

        private SwCoreConfig LoadLatestConfigFromPlayerPrefs()
        {
            var configStr = SwInfra.KeyValueStore.GetString(SwStoreKeys.LatestSuccessfulConfigResponse);;

            return DeserializeConfigJson(configStr);
        }
        
        private SwCoreConfig DeserializeConfigJson(string remoteConfigJsonString)
        {
            if (remoteConfigJsonString.SwIsNullOrEmpty())
            {
                return null;
            }
            try
            {
                return ParseConfig(remoteConfigJsonString);
            }
            catch (Exception ex)
            {
                SwInfra.Logger.LogException(ex, EWisdomLogType.Config, $"An error occured while trying to {nameof(ParseConfig)}");
            }
            
            return null;
        }

        protected virtual void NotifyInternalListeners(ESwConfigSource source)
        {
            if (_listeners != null && _listeners.Count > 0)
            {
                foreach (var listener in _listeners)
                {
                    if (listener is null) continue;
                    
                    if (listener.ListenerType.Item1 <= Timing && listener.ListenerType.Item2 >= Timing)
                    {
                        try
                        {
                            listener.OnConfigResolved(Config, this, _dynamicConfigManager, source);
                        }
                        catch (Exception e)
                        {
                            SwInfra.Logger.LogException(e, EWisdomLogType.Config, $"{nameof(NotifyInternalListeners)} - by one of the listeners {listener}");
                        }
                    }
                }
            }
        }

        private void SaveInitialConfigResponse(string responseText)
        {
            SwInfra.KeyValueStore
                .SetString(SwStoreKeys.InitialConfig, responseText)
                .Save();
        }

        private void SaveCachedConfigResponse(string responseText)
        {
            SwInfra.KeyValueStore
                   .SetString(SwStoreKeys.LatestSuccessfulConfigResponse, responseText)
                   .Save();
        }

        private void TryLoadLatestSuccessfulRemoteConfig()
        {
            var latestSuccessfulRemoteConfig = LoadLatestConfigFromPlayerPrefs();

            if (latestSuccessfulRemoteConfig != null)
            {
                SetNewConfig(latestSuccessfulRemoteConfig, null, EConfigStatus.Cached);
            }
        }

        private void SetNewConfig(SwCoreConfig configToSet, SwAbConfig ab, EConfigStatus newStatus, bool addLocalConfig = true)
        {
            SwInfra.Logger.Log(EWisdomLogType.Config, $"will set config: {configToSet}, ab: {ab}");
            Config = configToSet;
            SwInfra.Logger.Log(EWisdomLogType.Config, $"did set config: {configToSet}, ab: {ab}");
            
            Status = newStatus;
            _abInternalHandler.UpdateAb(ref _coreConfig, ab);
        }

        private SwCoreConfig TryParseRemoteConfig(SwWebResponse response)
        {
            if (!response.DidSucceed) return null;

            try
            {
                var remoteConfig = DeserializeConfigJson(response.Text);
                WebRequestError = response.error;
                
                return remoteConfig;
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogException(e, EWisdomLogType.Config, $"An error occured while trying {nameof(DeserializeConfigJson)}");
                return null;
            }
        }

        private void OnGameStarted()
        {
            _abInternalHandler.TryTrackInternalAbConcludedEvent();
            Timing = EConfigListenerType.EndOfGame;
        }

        private static bool IsNotInGameplayState(SwSystemState.EGameState state)
        {
            return state != SwSystemState.EGameState.Regular && state != SwSystemState.EGameState.Bonus && state != SwSystemState.EGameState.Tutorial && state != SwSystemState.EGameState.Time;
        }

        #endregion
    }
}
