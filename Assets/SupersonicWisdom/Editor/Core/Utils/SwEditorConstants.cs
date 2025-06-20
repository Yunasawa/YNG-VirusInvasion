namespace SupersonicWisdomSDK.Editor
{
    internal static class SwEditorConstants
    {
        #region --- Constants ---

        public const string BASE_FOLDER_NAME = "SupersonicWisdom";
        public const string SDK_NAME = BASE_FOLDER_NAME + "SDK";
        public const string ASSETS = "Assets";
        public const string EMPTY_STRINGIFIED_JSON = "{}";
        
        #if SW_STAGE_STAGE20
        public const string DEFAULT_STAGE_INTEGRATION_GUIDE_URL = "https://support.supersonic.com/hc/en-us/articles/30616685317396";
        #elif SW_STAGE_STAGE30
        public const string DEFAULT_STAGE_INTEGRATION_GUIDE_URL = "https://support.supersonic.com/hc/en-us/articles/30616692175764";
        #elif SW_STAGE_STAGE40
        public const string DEFAULT_STAGE_INTEGRATION_GUIDE_URL = "https://support.supersonic.com/hc/en-us/articles/30616683056020";
        #else
        public const string DEFAULT_STAGE_INTEGRATION_GUIDE_URL = "https://support.supersonic.com/hc/en-us/sections/30542303385108";
        #endif
        
        public const string DEFAULT_CHANGE_LOG_URL = "https://support.supersonic.com/hc/en-us/articles/30616692376852";
        public const string IAP_ASSEMBLY_FULL_NAME = "UnityEngine.Purchasing";
        public const string UGS_ANALYTICS_ASSEMBLY_FULL_NAME = "Unity.Services.Analytics";
        public const string UGS_REMOTE_CONFIG_ASSEMBLY_FULL_NAME = "Unity.Services.RemoteConfig";
        public const string UNITY_MOBILE_NOTIFICATIONS_ASSEMBLY_FULL_NAME = "Unity.Notifications";
        public const string UNITY_MOBILE_NOTIFICATIONS_PACKAGE_VERSION = "2.1.1";
        public const string UNITY_PURCHASING_PACKAGE_VERSION = "4.12.2";
        public const string UNITY_UGS_ANALYTICS_PACKAGE_VERSION = "5.1.0";
        public const string UNITY_UGS_REMOTE_CONFIG_PACKAGE_VERSION = "4.1.1";
        public const int FACEBOOK_ADVERTISER_ID_COLLECTION_STAGE_NUMBER_REQUIREMENT = 10;
        public const string AWS_ASSETS_URL = "https://assets.mobilegamestats.com/";
        public const string ASSETS_RESOURCES_PREFIX = "Assets/Resources/";
        public const string ASSETS_SUFFIX = ".asset";

        #endregion


        #region --- Inner Classes ---

        internal struct GamePlatformKey
        {
            #region --- Constants ---

            internal const string Android = "google_play";
            internal const string Ios = "app_store";
            internal const string IosChina = "app_store_cn";

            #endregion
        }

        internal struct SwKeys
        {
            #region --- Constants ---

            private const string SW_PREFIX = "Sw.";
            internal const string SHOULD_VERIFY_UPDATE = SW_PREFIX + "ShouldVerifyUpdate";
            internal const string IMPORTED_WISDOM_UPDATE_INFO = SW_PREFIX + "ImportedWisdomUpdateInfo";
            internal const string LAST_LOGIN_ALERT_TIMESTAMP = SW_PREFIX + "LastLoginAlertTimestamp";
            internal const string LAST_WISDOM_UPDATE_CHECKUP_TIMESTAMP = SW_PREFIX + "LastWisdomUpdateCheckupTimestamp";
            internal const string UPDATED_SDK_STAGE_NUMBER = SW_PREFIX + "UpdatedSdkStageNumber";
            internal const string OLD_SDK_STAGE_NUMBER = SW_PREFIX + "OldSdkStageNumber";
            internal const string UPDATED_SDK_VERSION_ID = SW_PREFIX + "UpdatedSdkVersionId";
            internal const string OLD_SDK_VERSION_ID = SW_PREFIX + "OldSdkVersionId";
            internal const string LAST_TIME_MINIMUM_EDITOR_VERSION_MESSAGE_SHOWN = SW_PREFIX + "LastTimeMinimumEditorVersionMessageShown";
            internal const string IMPORTED_UNITY_PACKAGE_FILE_PATH = SW_PREFIX + "ImportedUnityPackageBackupPath";
            internal const string IMPORTED_WISDOM_BACKUP_FOLDER_PATH = SW_PREFIX + "ImportedWisdomBackupFolderPath";
            internal const string LOGIN_LOCK = SW_PREFIX + "LoginLock";
            internal const string MIGRATED_TO_NEW_LOGIN_METHOD = SW_PREFIX + "MigratedToNewLoginMethod";

            #endregion
        }

        internal struct ErrorMessages
        {
            #region --- Constants ---

            public const string FAILED_TO_BACKUP = "Failed to backup current package content" + TERMINATING_UPDATE_PROCESS;
            public const string FAILED_TO_DELETE_CURRENT_VERSION = "Failed to delete all current package content" + TERMINATING_UPDATE_PROCESS;
            public const string FAILED_TO_IMPORT_PACKAGE = "Failed to import updated package" + TERMINATING_UPDATE_PROCESS;
            public const string FAILED_TO_VERIFY_SUCCESSFUL_UPDATE = "Failed to verify successful update" + RESTORING_FILE_SYSTEM;
            public const string FAILED_TO_UPDATE_DUE_TO_DOWNGRADE = "Stage update is available, but update failed due to new version being lower than current version, current version: {0}, new version: {1}" + CONTACT_SUPPORT;
            public const string FAILED_TO_UPDATE_DUE_TO_BLACKLISTING =
                "Version update is not allowed, current version is : {0}" +
                CONTACT_SUPPORT;
            private const string TERMINATING_UPDATE_PROCESS = "\nTerminating update process";
            private const string RESTORING_FILE_SYSTEM = "\nRestoring file system to previous state before update";
            private const string CONTACT_SUPPORT = "\nContact support for more information";

            #endregion
        }

        internal struct UI
        {
            #region --- Constants ---

            public const string ALERT = "Alert";
            public const string TAB = "Tab";
            public const string HIDE = "Hide";
            public const string SHOW = "Show";
            public const string ANDROID = "Android";
            public const string IOS = "iOS";
            public const string CHINA = "China";
            public const string APPLIED_BACKUP_RULES = "Applied Backup rules successfully";
            public const string APPLIED_NETWORK_DEBUG_CONFIGURATION = "Applied Network Debug configuration successfully";
            public const string CANNOT_APPLY_BACKUP_RULES = "Cannot apply backup rules.\nOpen the editor console to see failure reason.";
            public const string LOGIN_REQUIRED_TO_CHECK_UPDATES = "Cannot check for updates - Login is required";
            public const string CANT_CHECK_UPDATES_EMPTY_GAMES_LIST = "Can't check updates - Games list is empty, try to fix it with re-login";
            public const string CHOOSE_TITLE_MANUALLY = "Please choose your game from the drop down list to retrieve the credential IDs.";
            public const string CONFIGURATION_IS_UP_TO_DATE = "Configuration is up to date!";
            public const string CONFIGURATION_SUCCESSFULLY_COPIED = "Configuration successfully copied to settings";
            public const string DUPLICATE_ITEMS = "Looks like game platforms list has a duplicated item, actual list is: {0}";
            public const string DUPLICATE_PRODUCT = "There is a duplication of \"{0} \" in the \" Products\" list.\nPlease remove the duplication.";
            public const string FAILED_TO_CHECK_CURRENT_STAGE = "Failed to check current stage. Error: {0}";
            public const string FAILED_TO_DOWNLOAD_UNITY_PACKAGE = "Failed to download Unity package";
            public const string FAILED_TO_GENERATE_REMOTE_URL = "Unexpected error: remote URL failed to generate.";
            public const string ENABLE_AUTO_NOTIFICATION_PERMISSION_REQUEST_CHECKBOX_TITLE = "Prompt notifications authorization request on app launch";
            public const string WISDOM_AUTO_NOTIFICATION_PERMISSION_REQUEST_WARNING = "Enabling iOS automatic authorization request is not recommended for \"Wisdom managed notification\".\n\nAre you sure?";
            
            public static string FAILED_TO_UPDATE_WISDOM_PACKAGE(string errorMessageDetails) => "Failed to update Wisdom package. Details: " + errorMessageDetails;
            public const string INVALID_EMAIL_ADDRESS = "That's not a valid email address";
            public const string INTERNAL_WINDOW_PATH_PREFIX = "Window/SupersonicWisdom/";
            public const string TOOLS = "Tools";
            public const string CLEAR_SW_PREFS = "Clear Sw Json prefs";
            public const string INVALID_TOKEN = "The login token is invalid";
            public const string MISSING_TOKEN = "Missing token";
            public const string MISSING_UNITY_IAP = "Missing Unity IAP Package";
            public const string NEW_CREDENTIALS_IDS_IN_SETTINGS = "New credential IDs for {0} were populated to Wisdom SDK";
            public const string NO_NEED_TO_UPDATE = "No updates available";
            public const string PARAM_IS_MISSING = "The following param: \"{0}\" is missing.\nPlease add it.";
            public const string PLEASE_CHOOSE_TITLE = "Please choose title";
            public const string REACH_TECHNICAL_SUPPORT = "Please contact technical support support@supersonic.com or try again";
            public const string WISDOM_UPDATED_SUCCESSFULLY = "SupersonicWisdom updated successfully";
            public const string WISDOM_UPDATE_FAILED_DUE_TO_COMPILATION = "Wisdom update failed due to compilation!";
            public const string WISDOM_UPDATE_CHECK_FAILED = "Wisdom update check failed!";
            public const string SUPERSONIC_WISDOM = "Supersonic Wisdom";
            public const string SUPERSONIC_WISDOM_SDK = "Supersonic Wisdom SDK";
            public const string UPDATE_IN_PROGRESS = "An update is currently still in progress, wait until the current update is done before retry.";
            public const string VERIFY_GA_SETTINGS = "The following GameAnalytics Advanced Settings should be set to true:\nSend Version\nSubmit Errors\n";
            public const string WELCOME_MESSAGE = "Please go to settings window and Login with your Supersonic platform credentials to automatically retrieve the relevant credential IDs";
            public const string WELCOME_TITLE = "Welcome to Supersonic Wisdom SDK!";
            public const string UNITY_MINIMUM_VERSION_MESSAGE = "Your Unity Version is not supported by Wisdom.\nPlease update to Unity {0} LTS, or later.";
            public const string MINIMUM_EDITOR_VERSION_URL = "https://unity.com/releases/editor/archive#download-archive-{0}";
            public const string NATIVE = "Native";
            public const string SESSION_TEXT = "Session";
            public const string SESSION_ID = "Session Id";
            public const string IS_AVAILABLE = "Is Available";
            public const string INVOKE_BUTTON_TEXT = "Invoke";
            public const string IS_FLIGHT_MODE = "Is Flight Mode";
            public const string INTERNET_CONNECTION = "Internet Connection";
            public const string NATIVE_SIMULATOR_TOOL = "Native Simulator Tool";
            public const string INVOKE_SESSION_STARTED = "Invoke Session Started";
            public const string SETTING_PARAMETER_MISSING_ERROR = "Parameter missing error";
            public const string SETTING_PARAMETER_MISSING_ERROR_MESSAGE = "The following parameter is mandatory and is currently missing in your settings file: \n{0}";
            public const string TOKEN_TOOLTIP_TEXT = "To retrieve the 'Game Token', log in to the Supersonic platform -> select your game -> 'Game Details' -> 'Game Token'.";
            public const string TOKEN_MIGRATION_WARNING_MESSAGE = "Notice, we have improved our login security!\n" +
                                                                  "To get your new token-based login:\n" +
                                                                  "1. Visit the Supersonic platform\n" +
                                                                  "2. Select your game\n" +
                                                                  "3. Go to ‘Game Details’ > ‘Game Token’\n" +
                                                                  "4. Add it to your wisdom settings";
            public const string NO_INTERNET_ON_APP_OPEN_TOOLTIP = "0: Immediately block the user from playing the game.\n" +
                                                          "1+: Grace period (in seconds) before showing the No Internet popup";
            public const string ICON_QUESTION_MARK = "_Help";
            public const string CANNOT_APPLY_NETWORK_DEBUG_CONFIGURATION = "Cannot apply network debug configuration, please contact support if needed.";

            #endregion


            #region --- Members ---

            public static readonly string SupersonicWisdomSDKError = $"{SUPERSONIC_WISDOM_SDK}: Error";
            public static readonly string SupersonicWisdomSDKWarning = $"{SUPERSONIC_WISDOM_SDK}: Warning";

            #endregion


            #region --- Public Methods ---

            public static string DownloadingUpdatePackage(string downloadedSize, string percentagesDownloaded)
            {
                return $"Please do not compile your code during this process ({downloadedSize}, {percentagesDownloaded}%)";
            }

            #endregion


            #region --- Inner Classes ---

            internal struct ButtonTitle
            {
                #region --- Constants ---

                public const string AWESOME = "Awesome!";
                public const string CANCEL = "Cancel";
                public const string CLOSE = "Close";
                public const string GO_TO_SETTINGS = "Go to Settings";
                public const string LOGIN_NOW = "Login now";
                public const string OK = "OK";
                // ReSharper disable once InconsistentNaming
                public const string I_AM_SURE = "I'm sure";
                public const string REVERT = "Revert";
                public const string SET_TO_TRUE_AND_SAVE = "Set to true & Save";
                public const string THANKS = "Thanks";
                public const string UPDATE = "Update";

                #endregion
            }

            internal struct PrebuildMessages
            {
                #region --- Constants ---

                public const string FB_CLIENT_TOKEN_MISSING_MESSAGE = "We noticed that the Facebook client token is missing.\nAs of this version, It is mandatory.\nPlease fill it in.\nOn the Facebook Dashboard, navigate to Settings > Advanced > Security > Client token.";
                public const string ADMOB_APP_ID_MISSING_MESSAGE = "We noticed your Admbob ID is missing. \nFill it in the Wisdom settings or contact your POC at Supersonic.\n Supersonic Wisdom -> Edit Settings -> {0} -> Google AdMob ID";

                #endregion
            }

            #endregion
        }

        #endregion
    }

    public static class SwErrors
    {
        #region --- Enums ---

        public enum EMenu
        {
            CannotApplyDebuggableNetworkConfiguration = 1130,
            CannotApplyBackupRules = 1131,
            CannotApplyNetworkDebug
        }

        public enum ESettings
        {
            MissingTitles = 1104,
            DuplicatePlatform = 1105,
            ChooseTitle = 1106,
            MissingToken = 1107,
            InvalidToken = 1108,
        }

        public enum ESelfUpdate
        {
            ImportFailed = 1140,
            CheckCurrentStage = 1142,
            DownloadUpdatePackageFailed = 1143,
            CompilationInterference = 1144,
            CheckUpdatesFailed = 1145,
            CheckFailedDueToEmptyGamesList = 1146,
            InvalidUnityPackageRemoteUrl = 1355,
            DownloadUpdatePackageCanceled = 1356,
            DownloadChecksumMismatch = 1357,
            DowngradeDetected = 1358,
            VersionBlacklisted = 1359,
            InternalError = 1360,
        }

        public enum EBuild
        {
            MissingFacebookTokenSettings = 1400,
            MissingAdmobAppIdSettings = 1401,
        }

        public enum ECommunication
        {
            RequestFailed = 2140,
        }

        #endregion
    }
}