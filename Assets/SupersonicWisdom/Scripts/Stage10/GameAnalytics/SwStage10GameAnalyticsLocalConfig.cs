#if SW_STAGE_STAGE10_OR_ABOVE

using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwStage10GameAnalyticsLocalConfig : SwLocalConfig
    {
        public const string SW_GA_INIT_IS_ENABLED_KEY = "swGaInitIsEnabled";
        public const bool SW_GA_INIT_IS_ENABLED_VALUE = true;
        
        public override Dictionary<string, object> LocalConfigValues
        {
            get
            {
                return new Dictionary<string, object>()
                {
                    { SW_GA_INIT_IS_ENABLED_KEY, SW_GA_INIT_IS_ENABLED_VALUE },
                };
            }
        }
    }
}

#endif
