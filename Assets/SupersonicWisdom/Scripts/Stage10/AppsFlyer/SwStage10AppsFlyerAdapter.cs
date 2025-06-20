#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections;
using System.Collections.Generic;
using AppsFlyerSDK;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal static class SwStage10AppsFlyerConstants
    {
        #region --- Constants ---

        public const string APPSFLYER_INIT_INTERNAL_EVENT_NAME = "AppsFlyerInit";

        #endregion
    }

    internal class SwStage10AppsFlyerAdapter : ISwReadyEventListener, ISwAppsFlyerListener, ISwLocalConfigProvider, ISwAdapter, ISwStage10ConfigListener, ISwAsyncRunnable
    {
        #region --- Constants ---

        /// <summary>
        ///     This key is read by Wisdom Native. Do not rename it.
        /// </summary>
        private const string APPS_FLYER_DEV_KEY = "nYwfftoacbopmuszWBPGnd";
        private const string APPS_FLYER_CONVERSION_DATA_KEY = "AFConversionData";
        internal const string APPS_FLYER_INIT_TRIGGER_KEY = "af_init_complete";
        private const string NETTO_PLAYTIME_EVENT_NAME = "netto";
        private const string BRUTTO_PLAYTIME_EVENT_NAME = "brutto";
        private const float DELAY_TIMEOUT = 2f;

        #endregion


        #region --- Members ---

        protected readonly SwCoreTracker Tracker;

        protected readonly SwSettings Settings;
        protected bool DidApplyAppsFlyerAnonymize;
        protected bool DidAppsFlyerRespond;

        protected bool IsSwReady;
        protected bool? ShouldAnonymizeAppsFlyer = null;

        private readonly SwAppsFlyerEventDispatcher _eventDispatcher;
        private readonly SwStage10UserData _userData;
        private readonly SwTimerManager _timerManager;
        private bool _didInitAppsFlyer;
        private bool _didTrackConversionDataFail;
        private ISwAppsFlyerListener _listener;
        private ISwKeyValueStore _keyValueStore;
        private string _appsFlyerHostname;
        private string _conversionDataFailError;
        private string _sdkStatus;
        private EConfigStatus _configStatus;
        private SwTimerConfig _nettoPlaytimeTimerConfig;
        private SwTimerConfig _bruttoPlaytimeTimerConfig;
        private long _reportingCap = 0;
        private long _lastNettoEventTime = 0;
        private long _lastBruttoEventTime = 0;

        #endregion


        #region --- Properties ---

        public Tuple<EConfigListenerType, EConfigListenerType> ListenerType
        {
            get { return new Tuple<EConfigListenerType, EConfigListenerType>(EConfigListenerType.FinishWaitingForRemote, EConfigListenerType.FinishWaitingForRemote); }
        }

        #endregion


        #region --- Construction ---

        public SwStage10AppsFlyerAdapter(SwAppsFlyerEventDispatcher eventDispatcher, SwStage10UserData userData, SwSettings settings, SwCoreTracker tracker, SwTimerManager timerManager)
        {
            _eventDispatcher = eventDispatcher;
            Settings = settings;
            _userData = userData;
            Tracker = tracker;
            _timerManager = timerManager;
        }

        #endregion


        #region --- Public Methods ---

        public virtual void Init()
        {
            if (_didInitAppsFlyer) return;
            
            _didInitAppsFlyer = true;
            SwInfra.Logger.Log(EWisdomLogType.AppsFlyer, "Init");

            try
            {
                _keyValueStore = new SwUnmanagedPlayerPrefsStore();
                _eventDispatcher.AddListener(this);

                if (!string.IsNullOrEmpty(_appsFlyerHostname))
                {
                    AppsFlyer.setHost("", _appsFlyerHostname);
                }
                
                AppsFlyer.setIsDebug(Settings.enableDebug);
                AppsFlyer.initSDK(APPS_FLYER_DEV_KEY, Settings.IosAppId, _eventDispatcher);
                _timerManager.OnAllSessionPlaytimeTick += SendPlaytimeToAppsFlyer;
                SwInternalEvent.Invoke(SwStage10AppsFlyerConstants.APPSFLYER_INIT_INTERNAL_EVENT_NAME);
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogException(e, EWisdomLogType.AppsFlyer, $"Failed {nameof(Init)}");
                _sdkStatus = e.Message;
            }
        }

        public virtual void OnConfigResolved(ISwStage10InternalConfig config, ISwConfigManagerState state, ISwConfigAccessor accessor, ESwConfigSource source)
        {
            _configStatus = state.Status;
            var appsFlyerHostname = accessor?.GetValue(SwStage10AppsFlyerLocalConfig.APPSFLYER_DEFAULT_DOMAIN_KEY, SwStage10AppsFlyerLocalConfig.APPSFLYER_DEFAULT_DOMAIN_VALUE);
            var nettoPlaytimeRawConfig = accessor.GetValue(SwStage10AppsFlyerLocalConfig.APPSFLYER_TOTAL_NETTO_PLAYTIME_KEY, SwStage10AppsFlyerLocalConfig.APPSFLYER_TOTAL_NETTO_PLAYTIME_VALUE);
            var bruttoPlaytimeRawConfig = accessor.GetValue(SwStage10AppsFlyerLocalConfig.APPSFLYER_TOTAL_BRUTTO_PLAYTIME_KEY, SwStage10AppsFlyerLocalConfig.APPSFLYER_TOTAL_BRUTTO_PLAYTIME_VALUE);
            var gameplayReportingCap = accessor.GetValue(SwStage10AppsFlyerLocalConfig.APPSFLYER_TOTAL_GAMEPLAY_REPORTING_CAP_KEY, SwStage10AppsFlyerLocalConfig.APPSFLYER_TOTAL_GAMEPLAY_REPORTING_CAP_VALUE);
            SetupGameplayReportingConfig(nettoPlaytimeRawConfig, bruttoPlaytimeRawConfig, gameplayReportingCap);
            SetHost(appsFlyerHostname);
        }

        public Dictionary<string, string> CreateEventValues()
        {
            var organizationAdvertisingId = _userData.OrganizationAdvertisingId;
            var eventValues = new Dictionary<string, string>();

            if (SwUtils.System.IsRunningOnAndroid())
            {
                eventValues["appSetId"] = organizationAdvertisingId;
            }
            else if (SwUtils.System.IsRunningOnIos())
            {
                eventValues["idfv"] = organizationAdvertisingId;
            }

            eventValues["af_sw_stage"] = SwStageUtils.CurrentStage.sdkStage.ToString();

            return eventValues;
        }

        public virtual SwLocalConfig GetLocalConfig()
        {
            return new SwStage10AppsFlyerLocalConfig();
        }

        public SwAdapterData GetAdapterStatusAndVersion()
        {
            var adapterData = new SwAdapterData
            {
                adapterName = nameof(AppsFlyer),
                adapterStatus = _sdkStatus,
                adapterVersion = AppsFlyer.getSdkVersion(),
            };

            return adapterData;
        }

        public void onAppOpenAttribution(string attributionData)
        {
            AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
            SwInfra.Logger.Log(EWisdomLogType.AppsFlyer, $"{attributionData}");
        }

        public void onAppOpenAttributionFailure(string error)
        {
            AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
        }

        public void onConversionDataFail(string error)
        {
            AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
            _conversionDataFailError = error;
            TrackConversionDataFailIfNeeded();
        }

        public virtual void onConversionDataSuccess(string conversionData)
        {
            AppsFlyer.AFLog("didReceiveConversionData", conversionData);

            // This key is being read in wisdom native for it to be sent in all events
            _keyValueStore?.SetString(APPS_FLYER_CONVERSION_DATA_KEY, conversionData);
        }

        public void OnSwReady()
        {
            IsSwReady = true;
            TrackConversionDataFailIfNeeded();
        }

        public void SendEvent(string eventName, Dictionary<string, string> eventValues)
        {
            AppsFlyer.sendEvent(eventName, CreateEventValues().SwMerge(true, eventValues));
        }

        public IEnumerator Run()
        {
            Init();
            yield return SwInfra.TacSystem.InternalFireTriggersRoutine(APPS_FLYER_INIT_TRIGGER_KEY);
            yield return StartSdk();
        }

        public void OnTimeout() { }

        #endregion


        #region --- Private Methods ---

        private IEnumerator StartSdk()
        {
            if (!_didInitAppsFlyer) yield break;
            
            try { 
                AppsFlyer.startSDK();
                _userData.AppsFlyerId = AppsFlyer.getAppsFlyerId();
                _didInitAppsFlyer = true;
                _sdkStatus = _didInitAppsFlyer.ToString();
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogException(e, EWisdomLogType.AppsFlyer, $"Failed {nameof(StartSdk)}");
                _sdkStatus = e.Message;
            }
        
            SwInfra.Logger.Log(EWisdomLogType.AppsFlyer, "Started");
        }
        
        protected void AnonymizeIfNeeded()
        {
            if (!DidApplyAppsFlyerAnonymize && ShouldAnonymizeAppsFlyer != null)
            {
                try
                {
                    AppsFlyer.anonymizeUser((bool)ShouldAnonymizeAppsFlyer);
                    SwInfra.Logger.Log(EWisdomLogType.AppsFlyer, $"Privacy | {SwPrivacyPolicy.Gdpr} | AppsFlyer | anonymizeUser | {(bool)ShouldAnonymizeAppsFlyer}");
                    DidApplyAppsFlyerAnonymize = true;
                }
                catch (Exception e)
                {
                    SwInfra.Logger.LogException(e, EWisdomLogType.AppsFlyer, $"Failed {nameof(AnonymizeIfNeeded)}");
                }
            }
        }

        private void TrackConversionDataFailIfNeeded()
        {
            if (!_didTrackConversionDataFail && IsSwReady && !string.IsNullOrEmpty(_conversionDataFailError))
            {
                _didTrackConversionDataFail = true;
                Tracker.TrackInfraEvent("AFConversionFailure", _conversionDataFailError);
            }
        }

        private void SetHost(string appsflyerHostname)
        {
            if (appsflyerHostname.SwIsNullOrEmpty()) return;
            
            _appsFlyerHostname = appsflyerHostname;
            SwInfra.Logger.Log(EWisdomLogType.AppsFlyer, _appsFlyerHostname);
        }

        private void SendPlaytimeToAppsFlyer(float allSessionsNettoPlaytime, float allSessionsBruttoPlaytime)
        {
            var netPlaytimeSec = (long)Mathf.Floor(allSessionsNettoPlaytime);
            var brutPlaytimeSec = (long)Mathf.Floor(allSessionsBruttoPlaytime);

            SendPlaytimeIfRequired(netPlaytimeSec, _nettoPlaytimeTimerConfig, ref _lastNettoEventTime, NETTO_PLAYTIME_EVENT_NAME);
            SendPlaytimeIfRequired(brutPlaytimeSec, _bruttoPlaytimeTimerConfig, ref _lastBruttoEventTime, BRUTTO_PLAYTIME_EVENT_NAME);
        }
        
        private void SendPlaytimeIfRequired(long playtime, SwTimerConfig timerConfig, ref long lastEventTime, string type)
        {
            if (playtime > _reportingCap)
            {
                SwInfra.Logger.Log(EWisdomLogType.AppsFlyer, "Playtime exceeds reporting cap");
                _timerManager.OnAllSessionPlaytimeTick -= SendPlaytimeToAppsFlyer;
                
                return;
            }
            
            foreach (var (range, duration) in timerConfig)
            {
                if (!range.IsIn(playtime)) continue;
                
                if (playtime % duration == 0 && playtime != lastEventTime)
                {
                    lastEventTime = playtime;
                    var eventName = $"af_total_{type}_playtime{playtime}";
                    SwInfra.Logger.Log(EWisdomLogType.AppsFlyer, $"Sending playtime event: {eventName}");
                    SendEvent(eventName, new Dictionary<string, string>
                    {
                        ["af_playtime_type"] = type,
                        ["af_playtime_value"] = playtime.SwToString(),
                    });
                }
                 
                break;
            }
        }
        
        private void SetupGameplayReportingConfig(string rawNettoConfig, string rawBruttoConfig, string reportingCap)
        {
            _nettoPlaytimeTimerConfig = SwTimerConfigUtils.ParseTimerConfig(rawNettoConfig, (value) => (long)Mathf.Floor(value));
            _bruttoPlaytimeTimerConfig = SwTimerConfigUtils.ParseTimerConfig(rawBruttoConfig, (value) => (long)Mathf.Floor(value));
            _reportingCap = SwUtils.Parsing.TryParseLong(reportingCap);
            SwInfra.Logger.Log(EWisdomLogType.AppsFlyer, $"Netto: {_nettoPlaytimeTimerConfig}, Brutto: {_bruttoPlaytimeTimerConfig}, Cap: {_reportingCap}");
        }

        #endregion


        #region --- Event Handler ---

        public void OnAppsFlyerRequestResponse(object sender, EventArgs args)
        {
            if (args is AppsFlyerRequestEventArgs appsFlyerArgs && !DidAppsFlyerRespond)
            {
                DidAppsFlyerRespond = true;
                SwInfra.Logger.Log(EWisdomLogType.AppsFlyer, $"statusCode = {appsFlyerArgs.statusCode}");
                AnonymizeIfNeeded();
            }
        }

        #endregion
    }
}
#endif