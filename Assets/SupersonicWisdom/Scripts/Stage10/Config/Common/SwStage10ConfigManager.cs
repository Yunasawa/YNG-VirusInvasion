#if SW_STAGE_STAGE10_OR_ABOVE

using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwStage10ConfigManager : SwCoreConfigManager
    {
        #region --- Members ---
        
        private readonly List<ISwStage10ConfigListener> _listeners;

        #endregion


        #region --- Construction ---

        public SwStage10ConfigManager(ISwDynamicConfigManager dynamicConfigManager, SwCoreTracker tracker, SwStage10NativeAdapter swStage10NativeAdapter, SwWisdomRemoteConfigHandler wisdomRemoteConfigHandler, SwAbInternalHandler abInternalHandler) : base(dynamicConfigManager, tracker, swStage10NativeAdapter, wisdomRemoteConfigHandler, abInternalHandler)
        {
            _listeners = new List<ISwStage10ConfigListener>();
        }

        #endregion


        #region --- Public Methods ---

        public void AddListeners(List<ISwStage10ConfigListener> listeners)
        {
            _listeners.AddRange(listeners);
        }

        #endregion


        #region --- Private Methods ---
        
        protected override SwCoreConfig CreateLocalConfig(Dictionary<string, object> localConfigValues)
        {
            return new SwStage10Config(localConfigValues);;
        }

        public override SwCoreConfig ParseConfig(string configStr)
        {
            return SwUtils.JsonHandler.DeserializeObject<SwStage10Config>(configStr);
        }
        
        protected override void NotifyInternalListeners(ESwConfigSource source)
        {
            base.NotifyInternalListeners(source);

            if (Config is not SwStage10Config config) return;
            
            if (_listeners != null && _listeners.Count > 0)
            {
                foreach (var listener in _listeners)
                {
                    if (listener is null) continue;

                    if (listener.ListenerType.Item1 <= Timing && listener.ListenerType.Item2 >= Timing)
                    {
                        listener.OnConfigResolved(config, this, DynamicConfig, source);
                    }
                }
            }
        }

        #endregion
    }
}

#endif
