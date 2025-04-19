using System;
using System.Collections.Generic;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwWisdomRemoteConfigHandler : ISwDynamicConfigProvider, ISwNativeRequestListener, ISwCoreConfigListener, ISwLocalConfigProvider
    {
        #region --- Constants ---
        
        private const string SERVER_RESPONSE_EVENT_NAME = "ServerResponse";
        private const string REPORT_CONFIG_RESPONSE_TIME = "configResponseTime";
        private const string REPORT_CONFIG_RESPONSE_CODE = "configResponseCode";
        private const string REPORT_CONFIG_RESPONSE_ERROR = "configResponseError";
        private const string REPORT_CONFIG_RESPONSE_ITERATION = "configResponseIteration";
        private const string API_VERSION = "2";
        private const int CONFIG_DEFAULT_CAP = 10;
        private const int CONFIG_CONNECTION_TIMEOUT = 10000;
        private const int CONFIG_READ_TIMEOUT = 15000;
        private const string CONFIG_END_POINT = "https://{0}.config.mobilegamestats.com/";

        #endregion
        
        
        #region --- Fields ---
        
        private readonly ISwSettings _settings;
        private readonly SwCoreUserData _coreUserData;
        private readonly SwCoreTracker _tracker;
        private readonly SwCoreNativeAdapter _nativeAdapter;
        private readonly string _subdomain;

        private string _abGroup = string.Empty;
        private ISwConfigAccessor _configAccessor;
        private ISwConfigManager _configManager;
        private bool _configArrived;
        private List<Action<SwWebResponse>> _onSuccess;

        #endregion
        
        
        #region --- Construction ---

        public SwWisdomRemoteConfigHandler(ISwSettings settings, SwCoreUserData coreUserData, SwCoreTracker tracker, SwCoreNativeAdapter nativeAdapter)
        {
            _settings = settings;
            _coreUserData = coreUserData;
            _tracker = tracker;
            _nativeAdapter = nativeAdapter;
            _subdomain = _nativeAdapter.GetSubdomain();
            _onSuccess = new List<Action<SwWebResponse>>();
        }
        
        #endregion
        
        
        #region --- Properties ---
        
        public Action<Dictionary<string, object>> OnConfigUpdated { get; set; }

        public ESwConfigSource Source { get; } = ESwConfigSource.WisdomRemote;
        
        public Tuple<EConfigListenerType, EConfigListenerType> ListenerType
        {
            get
            {
                return new Tuple<EConfigListenerType, EConfigListenerType>(EConfigListenerType.Construction,
                    EConfigListenerType.FinishWaitingForRemote);
            }
        }
        
        public SwLocalConfig GetLocalConfig()
        {
            return new SwWisdomRemoteConfigHandlerLocalConfig();
        }
        
        #endregion
        
        
        #region --- Public Methods ---
        
        internal void InitHandler(ISwConfigManager configManager)
        {
            _configManager ??= configManager;
            OnConfigUpdated?.Invoke(GetStoredConfig());
        }

        public Dictionary<string, object> GetConfig()
        {
            return new Dictionary<string, object>();
        }

        public void FetchRemoteConfig(params Action<SwWebResponse>[] onSuccess)
        {
            if (_configArrived) return;
            
            _onSuccess.AddRange(onSuccess);

            SwInfra.Logger.Log(EWisdomLogType.Config, "Send remote config request");

            var bodyJsonString = SwUtils.JsonHandler.SerializeObject(CreatePayload());
            
            _nativeAdapter.SendRequest(CONFIG_END_POINT.Format(_subdomain), bodyJsonString, this, "", CONFIG_CONNECTION_TIMEOUT, CONFIG_READ_TIMEOUT, CONFIG_DEFAULT_CAP);
        }

        public void OnSuccess(SwWebResponse response)
        {
            if (_configArrived) return;

            SwInfra.Logger.Log(EWisdomLogType.Config);
            _configArrived = true;

            var parsedRemoteConfig = DeserializeConfigJson(response.Text);

            if (parsedRemoteConfig == null)
            {
                return;
            }
            
            OnConfigUpdated?.Invoke(parsedRemoteConfig.DynamicConfig);
            
            foreach (var callback in _onSuccess)
            {
                callback.Invoke(response);
            }

            TrackResponse(response);
            _onSuccess.Clear();
            _configArrived = false;
        }

        public void OnFail(SwWebResponse response)
        {
            SwInfra.Logger.Log(EWisdomLogType.Config);
            TrackResponse(response);
        }

        public void OnIteration(SwWebResponse response)
        {
            SwInfra.Logger.Log(EWisdomLogType.Config);

            if (_configAccessor != null && !_configAccessor.GetValue(SwWisdomRemoteConfigHandlerLocalConfig.SHOULD_REPORT_CONFIG_ITERATION_KEY, SwWisdomRemoteConfigHandlerLocalConfig.SHOULD_REPORT_CONFIG_ITERATION_VALUE))
            {
                return;
            }

            TrackResponse(response);
        }
        
        public void OnConfigResolved(ISwCoreInternalConfig swInternalConfig, ISwConfigManagerState state, ISwConfigAccessor swConfigAccessor, ESwConfigSource source)
        {
            if (source == ESwConfigSource.UgsRemote) return;
            
            _abGroup = swInternalConfig.Ab.group;
            _configAccessor = swConfigAccessor;
            _configManager = (ISwConfigManager)state;
        }
        
        #endregion


        #region --- Private Methods ---
        
        private void TrackResponse(SwWebResponse response)
        {
            var dict = new Dictionary<string, object>
            {
                { REPORT_CONFIG_RESPONSE_TIME, response.time },
                { REPORT_CONFIG_RESPONSE_CODE, response.code },
                { REPORT_CONFIG_RESPONSE_ERROR, response.error },
                { REPORT_CONFIG_RESPONSE_ITERATION, response.iteration },
            };
            
            _tracker?.TrackEventWithParams(SERVER_RESPONSE_EVENT_NAME, dict);
        }
        
        private SwCoreConfig DeserializeConfigJson(string remoteConfigJsonString)
        {
            if (string.IsNullOrEmpty(remoteConfigJsonString))
            {
                return null;
            }
            try
            {
                return _configManager?.ParseConfig(remoteConfigJsonString);
            }
            catch (Exception ex)
            {
                SwInfra.Logger.LogException(ex, EWisdomLogType.Config, $"An error occurred while trying to parse config");
            }
            
            return null;
        }
        
        private Dictionary<string, object> GetStoredConfig()
        {
            var initialConfig = LoadConfigFromPlayerPrefs(EStoredConfig.Initial);
            var latestConfig = LoadConfigFromPlayerPrefs(EStoredConfig.Latest);
            
            if (initialConfig == null && latestConfig == null)
            {
                return new Dictionary<string, object>();
            }
            
            return latestConfig?.DynamicConfig ?? initialConfig?.DynamicConfig;
        }

        private SwCoreConfig LoadConfigFromPlayerPrefs(EStoredConfig type)
        {
            var configStr = string.Empty;
            
            switch (type)
            {
                case EStoredConfig.Initial:
                    configStr = SwInfra.KeyValueStore.GetString(SwStoreKeys.InitialConfig);
                    break;
                
                case EStoredConfig.Latest:
                    configStr = SwInfra.KeyValueStore.GetString(SwStoreKeys.LatestSuccessfulConfigResponse);
                    break;
            }

            return DeserializeConfigJson(configStr);
        }

        protected virtual SwRemoteConfigRequestPayload CreatePayload()
        {
            var organizationAdvertisingId = _coreUserData.OrganizationAdvertisingId;

            return new SwRemoteConfigRequestPayload
            {
                bundle = _coreUserData.BundleIdentifier,
                gameId = _settings.GetGameId(),
                os = _coreUserData.Platform,
                osver = SystemInfo.operatingSystem,
                uuid = _coreUserData.Uuid,
                session = _coreUserData.ImmutableUserState().SessionId,
                device = SystemInfo.deviceModel,
                version = Application.version,
                sdkVersion = SwConstants.SDK_VERSION,
                sdkVersionId = SwConstants.SdkVersionId,
                stage = SwStageUtils.CurrentStage.sdkStage,
                sysLang = Application.systemLanguage.ToString(),
                isNew = _coreUserData.IsNew ? "1" : "0",
                apiVersion = API_VERSION,
                installSdkVersion = _coreUserData.InstallSdkVersion,
                installSdkVersionId = _coreUserData.InstallSdkVersionId,
                abVariant = _abGroup,
#if UNITY_IOS
                idfv = organizationAdvertisingId,
#endif
#if UNITY_ANDROID
                appSetId = organizationAdvertisingId,
#endif
            };
        }
        
        #endregion
    }
}
