using System;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    /// <summary>
    /// This class is responsible for handling timeout logic.
    /// Used and managed by <see cref="SwAsyncFlow"/>.
    /// </summary>
    internal class SwTimeoutHandler : ISwLocalConfigProvider, ISwCoreConfigListener
    {
        #region --- Constants ---

        internal const float GRACE_TIME_AFTER_TIMEOUT = 0.2f;
        
        #endregion
        
        
        #region --- Members ---
        
        private bool _hasStarted;
        private readonly ISwTimer _timer;

        #endregion 

        
        #region --- Properties ---
        
        internal bool IsTimerPaused
        {
            get { return _timer.IsPaused; }
        }

        internal float TimeUntilTimeout
        {
            get { return _timer.Remaining; }
        }
        
        public Tuple<EConfigListenerType, EConfigListenerType> ListenerType
        {
            get { return new Tuple<EConfigListenerType, EConfigListenerType>(EConfigListenerType.Construction, EConfigListenerType.FinishWaitingForRemote); }
        }

        #endregion

        
        #region --- Constructors ---
        
        internal SwTimeoutHandler(GameObject mono)
        {
            _timer = SwTimer.Create(mono, nameof(SwTimeoutHandler), SwTimeoutHandlerLocalConfig.SW_INIT_TIMEOUT_TIME_IN_SEC_VALUE, true);
        }
        
        #endregion
        
        
        #region --- Public Methods ---

        internal void StartTimer()
        {
            if (_hasStarted)
            {
                SwInfra.Logger.LogWarning(EWisdomLogType.Timeout, "Timeout handler has already started.");
                return;
            }

            _hasStarted = true;
            _timer.StartTimer();
        }
        
        internal void ResetTimer()
        {
            _timer.PauseTimer(true);
            _hasStarted = false;
        }
        
        internal void PauseTimer()
        {
            _timer.PauseTimer(true);
            SwInfra.Logger.Log(EWisdomLogType.Timeout, "Timeout timer paused at: " + TimeUntilTimeout + " seconds left.");
        }
        
        internal void ResumeTimer()
        {
            _timer.ResumeTimer();
            SwInfra.Logger.Log(EWisdomLogType.Timeout, "Timeout timer resumed at: " + TimeUntilTimeout + " seconds left.");
        }
        
        public SwLocalConfig GetLocalConfig()
        {
            return new SwTimeoutHandlerLocalConfig();
        }
        
        public void OnConfigResolved(ISwCoreInternalConfig internalConfig, ISwConfigManagerState state, ISwConfigAccessor configAccessor, ESwConfigSource source)
        {
            if (_timer == null || state.Status < EConfigStatus.Remote) return;

            var configTimeout = configAccessor.GetValue(SwTimeoutHandlerLocalConfig.SW_INIT_TIMEOUT_TIME_IN_SEC_KEY, SwTimeoutHandlerLocalConfig.SW_INIT_TIMEOUT_TIME_IN_SEC_VALUE);

            if (_timer.DidFinish)
            {
                SwInfra.Logger.Log(EWisdomLogType.Timeout, "Timer has already finished.");
                return;
            }

            AdjustTimeoutTimer(configTimeout);
        }

        #endregion
        
        
        #region --- Private Methods ---
        
        private void AdjustTimeoutTimer(float configTimeout)
        {
            if (!_timer.IsEnabled) return;
                
            if (configTimeout > SwTimeoutHandlerLocalConfig.SW_INIT_TIMEOUT_TIME_IN_SEC_VALUE)
            {
                var excessTime = configTimeout - SwTimeoutHandlerLocalConfig.SW_INIT_TIMEOUT_TIME_IN_SEC_VALUE;
                _timer.SetDuration(_timer.Remaining + excessTime);
                SwInfra.Logger.Log(EWisdomLogType.Timeout, $"Extended timer by {excessTime} seconds.");
            }
        }
        
        #endregion
    }
}