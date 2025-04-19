#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwStage10InstallSourceHandlerLocalConfig : SwLocalConfig
    {
        #region --- Constants ---

        public const string INSTALL_SOURCE_POPUP_ENABLED_KEY = "swInstallSourcePopupEnabled";
        public const bool INSTALL_SOURCE_POPUP_ENABLED_VALUE = false;
        public const string INSTALL_SOURCE_POPUP_TITLE_KEY = "swInstallSourcePopupTitle";
        public const string INSTALL_SOURCE_POPUP_TITLE_VALUE = "Oops! You've got the unofficial version";
        public const string INSTALL_SOURCE_POPUP_BODY_KEY = "swInstallSourcePopupBody";
        public const string INSTALL_SOURCE_POPUP_BODY_VALUE = "Click here to download the original one.";

        #endregion


        #region --- Properties ---

        public override Dictionary<string, object> LocalConfigValues
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { INSTALL_SOURCE_POPUP_ENABLED_KEY, INSTALL_SOURCE_POPUP_ENABLED_VALUE },
                    { INSTALL_SOURCE_POPUP_TITLE_KEY, INSTALL_SOURCE_POPUP_TITLE_VALUE },
                    { INSTALL_SOURCE_POPUP_BODY_KEY, INSTALL_SOURCE_POPUP_BODY_VALUE },
                };
            }
        }

        #endregion
    }
}
#endif