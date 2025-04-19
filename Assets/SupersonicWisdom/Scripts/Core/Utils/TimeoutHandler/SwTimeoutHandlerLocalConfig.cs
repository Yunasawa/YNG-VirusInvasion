using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwTimeoutHandlerLocalConfig : SwLocalConfig
    {
        #region --- Constants ---
        
        internal const string SW_INIT_TIMEOUT_TIME_IN_SEC_KEY = "swInitTimeoutTimeInSeconds";
        internal const float SW_INIT_TIMEOUT_TIME_IN_SEC_VALUE = 
            #if UNITY_EDITOR 
            // for debugging purposes
            float.MaxValue;
            #else
            5f;
            #endif
        
        #endregion
        
        
        #region --- Properties ---
        
        public override Dictionary<string, object> LocalConfigValues {             
            get {
                return new Dictionary<string, object>
                {
                    { SW_INIT_TIMEOUT_TIME_IN_SEC_KEY, SW_INIT_TIMEOUT_TIME_IN_SEC_VALUE },
                };
            }
        }
        
        #endregion
    }
}