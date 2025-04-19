using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwTimerManager : ISwReadyEventListener, ISwTrackerDataProvider, ISwScriptLifecycleListener, ISwSessionListener
    {
        #region --- Constants ---

        private const int PLAYTIME_TICK_INTERVAL = 1;
        private const int SAVE_PLAYTIME_INTERVAL = 5;
        private const string ACCUMULATED_SESSIONS_PLAYTIME = "AccumulatedSessionsPlaytime";
        private const string ACCUMULATED_SESSION_NOT_IN_PLAYTIME = "AccumulatedSessionsNotInPlaytime";
        
        private const string MEGA_NETO_PLAYTIME_KEY = "megaNetoPlaytime";
        private const string TOTAL_NETO_PLAYTIME_KEY = "totalNetoPlaytime";
        private const string MEGA_PLAYTIME_KEY = "megaPlaytime";
        internal const string PLAYTIME_TICK_TRIGGER_KEY = "playtime_tick";

        #endregion


        #region --- Members ---

        protected readonly ISwTimer _currentSessionPlaytimeNetoStopWatch;
        protected readonly ISwTimer _currentSessionAdsStopWatch;
        private readonly ISwTimer _currentSessionPlaytimeStopWatch;
        private readonly float _previousSessionsPlaytimeInSeconds;
        private readonly float _previousSessionsNotInPlaytimeInSeconds;
        private readonly ISwMonoBehaviour _mono;

        #endregion
        
        
        #region --- Events ---

        internal event OnAllSessionsTick OnAllSessionPlaytimeTick;
        internal delegate void OnAllSessionsTick(float allSessionsNettoElapsed, float allSessionsBruttoElapsed);
        
        #endregion
        
        
        #region --- Properties ---

        public ISwTimerListener CurrentSessionPlaytimeNetoStopWatch
        {
            get { return _currentSessionPlaytimeNetoStopWatch; }
        }
        
        public ISwTimerListener CurrentSessionAdsStopWatch
        {
            get { return _currentSessionAdsStopWatch; }
        }

        public float CurrentSessionPlaytimeElapsed
        {
            get { return CurrentSessionPlaytimeStopWatch?.Elapsed ?? -1f; }
        }

        public float AllSessionsPlaytimeNeto
        {
            get { return _previousSessionsPlaytimeInSeconds + CurrentSessionPlaytimeNetoStopWatch?.Elapsed ?? 0; }
        }
        
        public float AllSessionPlaytimeBrutto
        {
            get { return _previousSessionsNotInPlaytimeInSeconds + CurrentSessionPlaytimeNetoStopWatch?.Elapsed + CurrentSessionAdsStopWatch?.Elapsed ?? 0; }
        }
        
        internal float CurrentSessionPlaytime => CurrentSessionPlaytimeStopWatch?.Elapsed + CurrentSessionAdsStopWatch?.Elapsed ?? -1f;

        internal ISwTimerListener CurrentSessionPlaytimeStopWatch
        {
            get { return _currentSessionPlaytimeStopWatch; }
        }
        
        protected internal float CurrentSessionPlaytimeNeto => CurrentSessionPlaytimeNetoStopWatch?.Elapsed ?? -1f;
        
        #endregion


        #region --- Construction ---

        public SwTimerManager(ISwMonoBehaviour mono)
        {
            _mono = mono;
            _currentSessionPlaytimeNetoStopWatch = SwStopWatch.Create(mono.GameObject, $"{ETimers.CurrentSessionNetoPlaytimeMinutes}", true, PLAYTIME_TICK_INTERVAL);
            _currentSessionAdsStopWatch = SwStopWatch.Create(mono.GameObject, $"{ETimers.CurrentSessionAdsTimer}", false, PLAYTIME_TICK_INTERVAL);
            _currentSessionPlaytimeStopWatch = SwStopWatch.Create(mono.GameObject, $"{ETimers.CurrentSessionPlaytimeMinutes}", false, PLAYTIME_TICK_INTERVAL);
            _currentSessionPlaytimeStopWatch.OnTickEvent += NotifyAllSessionsPlaytimeTick;
            float.TryParse(SwInfra.KeyValueStore.GetString(ACCUMULATED_SESSIONS_PLAYTIME, "0"), NumberStyles.Float, CultureInfo.InvariantCulture, out _previousSessionsPlaytimeInSeconds);
            float.TryParse(SwInfra.KeyValueStore.GetString(ACCUMULATED_SESSION_NOT_IN_PLAYTIME, "0"), NumberStyles.Float, CultureInfo.InvariantCulture, out _previousSessionsNotInPlaytimeInSeconds);
            _mono.ApplicationFocusEvent += OnApplicationFocus;
            _mono.ApplicationPausedEvent += OnApplicationPaused;
        }

        ~SwTimerManager()
        {
            _mono.ApplicationFocusEvent -= OnApplicationFocus;
            _mono.ApplicationPausedEvent -= OnApplicationPaused;
        }

        (SwJsonDictionary dataDictionary, IEnumerable<string> keysToEncrypt) ISwTrackerDataProvider.ConditionallyAddExtraDataToTrackEvent(SwCoreUserData coreUserData, string eventName)
        {
            var shouldTrackPastMinimumSdkVersion = SwTrackerConstants.ShouldTrackPastMinimumSdkVersion(coreUserData, SwTrackerConstants.EMinimumSdkVersion.Sdk_7_7_18);
            var extraDataToSendTracker = new SwJsonDictionary
            {
                { MEGA_NETO_PLAYTIME_KEY, (int)Mathf.Round(CurrentSessionPlaytimeNeto) },
                { MEGA_PLAYTIME_KEY, (int)Mathf.Round(CurrentSessionPlaytime) },
            };

            if (shouldTrackPastMinimumSdkVersion)
            {
                extraDataToSendTracker.Add(TOTAL_NETO_PLAYTIME_KEY, (int)Mathf.Round(AllSessionsPlaytimeNeto));
            }

            return (extraDataToSendTracker, KeysToEncrypt(shouldTrackPastMinimumSdkVersion));

            IEnumerable<string> KeysToEncrypt(bool shouldEncryptAll)
            {
                yield break; 
            }
        }

        #endregion


        #region --- Public Methods ---

        public void OnSwReady()
        {
            _currentSessionPlaytimeStopWatch.StartTimer();
            _currentSessionPlaytimeNetoStopWatch.StartTimer();
            
            SaveAndTriggerPlaytimeRepeating();
        }
        
        public virtual void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                _currentSessionPlaytimeStopWatch.PauseTimer(true);
            }
            else
            {
                _currentSessionPlaytimeStopWatch.ResumeTimer();
            }
        }

        public void OnApplicationQuit() { }

        public void OnAwake() { }

        public void OnStart() { }

        public void OnUpdate() { }

        #endregion


        #region --- Private Methods ---
        
        private void SaveAndTriggerPlaytimeRepeating()
        {
            SwInfra.CoroutineService.RunActionEndlessly(SaveAndTriggerPlaytime, SAVE_PLAYTIME_INTERVAL, () => false);
        }

        private void SaveAndTriggerPlaytime()
        {
            // F symbolizes float format - #.## 
            SwInfra.KeyValueStore.SetString(ACCUMULATED_SESSIONS_PLAYTIME, AllSessionsPlaytimeNeto.ToString("F", CultureInfo.InvariantCulture), save: true);
            SwInfra.KeyValueStore.SetString(ACCUMULATED_SESSION_NOT_IN_PLAYTIME, AllSessionPlaytimeBrutto.ToString("F", CultureInfo.InvariantCulture), save: true);
            // Make sure logs a double of 5
            SwInfra.TacSystem.InternalFireTriggers(PLAYTIME_TICK_TRIGGER_KEY);
        }

        #endregion


        #region --- Event Handler ---
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus) return;

            SaveAndTriggerPlaytime();
        }

        private void OnApplicationPaused(bool isPaused)
        {
            // On iOS, the OnApplicationFocus event is not triggered, so we need to save the playtime here as well
#if UNITY_IOS
            if(!isPaused) return;
            
            SaveAndTriggerPlaytime();
#endif
        }
        
        public void OnSessionEnded(string sessionId)
        {
            // Ending a session in Android sometimes doesn't trigger the OnApplicationPaused event, so we need to save the playtime here as well
            SaveAndTriggerPlaytime();
        }

        public void OnSessionStarted(string sessionId) { }
        
        private void NotifyAllSessionsPlaytimeTick(float elapsed, float remaining)
        {
            OnAllSessionPlaytimeTick?.Invoke(AllSessionsPlaytimeNeto, AllSessionPlaytimeBrutto);
        }

        #endregion
    }

    internal enum ETimers
    {
        CurrentSessionPlaytimeMinutes,
        CurrentSessionNetoPlaytimeMinutes,
        CurrentSessionAdsTimer,
    }
}