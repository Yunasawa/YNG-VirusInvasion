#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwStage10AppsFlyerLocalConfig : SwLocalConfig
    {
        #region --- Constants ---

        public const string APPSFLYER_DEFAULT_DOMAIN_VALUE = "appsflyersdk.com";
        public const string APPSFLYER_DEFAULT_DOMAIN_KEY = "appsFlyerDomain";
        public const string APPSFLYER_TOTAL_NETTO_PLAYTIME_VALUE = "";
        public const string APPSFLYER_TOTAL_NETTO_PLAYTIME_KEY = "swAppsFlyerTotalNettoPlayTime";
        public const string APPSFLYER_TOTAL_BRUTTO_PLAYTIME_VALUE = "";
        public const string APPSFLYER_TOTAL_BRUTTO_PLAYTIME_KEY = "swAppsFlyerTotalBruttoPlayTime";
        public const string APPSFLYER_TOTAL_GAMEPLAY_REPORTING_CAP_VALUE = "400";
        public const string APPSFLYER_TOTAL_GAMEPLAY_REPORTING_CAP_KEY = "swAppsFlyerTotalGameplayReportingCap";

        #endregion


        #region --- Properties ---

        public override Dictionary<string, object> LocalConfigValues
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { APPSFLYER_DEFAULT_DOMAIN_KEY, APPSFLYER_DEFAULT_DOMAIN_VALUE },
                    { APPSFLYER_TOTAL_NETTO_PLAYTIME_KEY, APPSFLYER_TOTAL_NETTO_PLAYTIME_VALUE },
                    { APPSFLYER_TOTAL_BRUTTO_PLAYTIME_KEY, APPSFLYER_TOTAL_BRUTTO_PLAYTIME_VALUE },
                    { APPSFLYER_TOTAL_GAMEPLAY_REPORTING_CAP_KEY, APPSFLYER_TOTAL_GAMEPLAY_REPORTING_CAP_VALUE },
                };
            }
        }

        #endregion
    }
}
#endif