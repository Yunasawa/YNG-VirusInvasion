#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections;
using GameAnalyticsSDK;
using GameAnalyticsSDK.Setup;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    public delegate void OnGameAnalyticsInit ();

    internal static class SwStage10GameAnalyticsConstants
    {
        #region --- Constants ---

        public const string GAME_ANALYTICS_INIT_INTERNAL_EVENT_NAME = "GameAnalyticsInit";
        public const string GAME_ANALYTICS_INIT_SKIPPED_INTERNAL_EVENT_NAME = "GameAnalyticsInitSkipped";

        #endregion
    }

    internal class SwStage10GameAnalyticsAdapter : ISwGameProgressionListener, ISwAdapter, ISwAsyncRunnable, ISwStage10ConfigListener, ISwLocalConfigProvider
    {
        #region --- Events ---

        internal event OnGameAnalyticsInit OnGameAnalyticsInitEvent;

        #endregion


        #region --- Properties ---

        public bool DidInitializeGameAnalytics { get; private set; }
        public bool IsGameAnalyticsInitialized { get; private set; }

        public Tuple<EConfigListenerType, EConfigListenerType> ListenerType
        {
            get
            {
                return new Tuple<EConfigListenerType, EConfigListenerType>(EConfigListenerType.Construction, EConfigListenerType.EndOfGame);
            }
        }

        #endregion


        #region --- Members ---

        private string _sdkStatus;
        private bool _gaInitIsEnabled;

        #endregion


        #region --- Public Methods ---

        protected void Init()
        {
            if (DidInitializeGameAnalytics)
            {
                return;
            }

            DidInitializeGameAnalytics = true;

            if (ShouldInit() || Application.isEditor)
            {
                GameAnalytics.Initialize();
                IsGameAnalyticsInitialized = true;
                AfterInit(true);
            }
            else
            {
                AfterInit(false);
            }
        }

        public void OnLevelCompleted(ESwLevelType levelType, long level, string customString, long attempts, long revives)
        {
            if (!IsGameAnalyticsInitialized) return;
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, GetLevelName(level), levelType.ToString(), customString);
        }

        public void OnTimeBasedGameStarted () { }
        
        public void OnLevelFailed(ESwLevelType levelType, long level, string customString, long attempts, long revives)
        {
            if (!IsGameAnalyticsInitialized) return;
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, GetLevelName(level), levelType.ToString(), customString);
        }

        public void OnLevelRevived(ESwLevelType levelType, long level, string customString, long attempts, long revives) { }

        public void OnLevelSkipped(ESwLevelType levelType, long level, string customString, long attempts, long revives)
        {
            if (!IsGameAnalyticsInitialized) return;
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, GetLevelName(level),  levelType.ToString(), customString);
        }

        public void OnLevelStarted(ESwLevelType levelType, long level, string customString, long attempts, long revives)
        {
            if (!IsGameAnalyticsInitialized) return;
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, GetLevelName(level),  levelType.ToString(), customString);
        }

        public void OnLevelContinued(ESwLevelType levelType, long level, string customString, long attempts, long revives) { }

        public void OnMetaStarted(string customString)
        {
            if (!IsGameAnalyticsInitialized) return;
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, ESwGameplayType.Meta.ToString(), customString);
        }
        
        public void OnMetaEnded(string customString)
        {
            if (!IsGameAnalyticsInitialized) return;
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, ESwGameplayType.Meta.ToString(), customString);
        }

        public void SendDesignEvent(string eventName, float eventType)
        {
            if (!IsGameAnalyticsInitialized) return;
            GameAnalytics.NewDesignEvent(eventName, eventType);
        }
        
        public SwAdapterData GetAdapterStatusAndVersion()
        {
            var adapterData = new SwAdapterData
            {
                adapterName = nameof(GameAnalytics),
                adapterStatus = _sdkStatus,
                adapterVersion = Settings.VERSION
            };

            return adapterData;
        }
        
        public IEnumerator Run()
        {
            Init();
            yield break;
        }
        
        public void OnTimeout() { }

        public void OnConfigResolved(ISwStage10InternalConfig internalConfig, ISwConfigManagerState state, ISwConfigAccessor swConfigAccessor, ESwConfigSource source)
        {
            _gaInitIsEnabled = swConfigAccessor.GetValue(SwStage10GameAnalyticsLocalConfig.SW_GA_INIT_IS_ENABLED_KEY, SwStage10GameAnalyticsLocalConfig.SW_GA_INIT_IS_ENABLED_VALUE);
        }

        public SwLocalConfig GetLocalConfig()
        {
            return new SwStage10GameAnalyticsLocalConfig();
        }

        #endregion


        #region --- Private Methods ---

        protected virtual void AfterInit(bool didInit)
        {
            _sdkStatus = didInit.ToString();
            
            if (didInit)
            {
                SwInfra.Logger.Log(EWisdomLogType.GameAnalytics, "init");
                SwInternalEvent.Invoke(SwStage10GameAnalyticsConstants.GAME_ANALYTICS_INIT_INTERNAL_EVENT_NAME);

                try
                {
                    OnGameAnalyticsInitEvent?.Invoke();
                }
                catch (Exception e)
                {
                    SwInfra.Logger.LogException(e, EWisdomLogType.GameAnalytics, "OnGameAnalyticsInitEvent Invoke Error");
                    _sdkStatus = e.Message;
                }
            }
            else
            {
                SwInternalEvent.Invoke(SwStage10GameAnalyticsConstants.GAME_ANALYTICS_INIT_SKIPPED_INTERNAL_EVENT_NAME);
                
                //GA still runs FPS submissions even when not initialize and it throws an error
                //so we want to override the settings even if it's 'true' in the serialize object
                GameAnalytics.SettingsGA.SubmitFpsAverage = false;
                GameAnalytics.SettingsGA.SubmitFpsCritical = false;
            }
        }

        protected virtual bool ShouldInit ()
        {
            return _gaInitIsEnabled;
        }

        private string GetLevelName(long level)
        {
            return "level_" + level.ToString("D3"); // e.g "level_007"
        }

        #endregion
    }
}
#endif