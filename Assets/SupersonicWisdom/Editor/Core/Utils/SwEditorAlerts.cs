using System.Collections.Generic;
using UnityEditor;

namespace SupersonicWisdomSDK.Editor
{
    public static class SwEditorAlerts
    {
        #region --- Public Methods ---

        #region --- Package import ---

        public static void AlertWelcomeMessage()
        {
            if (EditorUtility.DisplayDialog(SwEditorConstants.UI.WELCOME_TITLE, SwEditorConstants.UI.WELCOME_MESSAGE, SwEditorConstants.UI.ButtonTitle.GO_TO_SETTINGS))
            {
                SwEditorUtils.OpenSettings();
            }
        }

        #endregion

        #endregion


        #region --- General ---

        public static bool Alert(string message, string okText, string cancel = "")
        {
            return EditorUtility.DisplayDialog(SwEditorConstants.UI.SUPERSONIC_WISDOM_SDK, message, okText, cancel);
        }

        public static bool AlertWarning(string message, string okText, string cancel = "")
        {
            return EditorUtility.DisplayDialog(SwEditorConstants.UI.SupersonicWisdomSDKWarning, message, okText, cancel);
        }

        public static bool AlertError(string message, long code, string buttonText, string cancel = "")
        {
            return EditorUtility.DisplayDialog(SwEditorConstants.UI.SupersonicWisdomSDKError, $"{message}\n\nError code: {(int)code}", buttonText, cancel);
        }
        
        public static void CustomAlert(string message, string okText, float interactiveDelay = 0f, string cancel = "")
        { 
            SwEditorCustomAlertWindow.ShowAlert(message, okText, interactiveDelay, cancel);
        }

        #endregion


        #region --- Settings ---

        public static void AlertMissingTitles()
        {
            SwEditorUtils.CallOnNextUpdate(() => { AlertError(SwEditorConstants.UI.REACH_TECHNICAL_SUPPORT, (int)SwErrors.ESettings.MissingTitles, SwEditorConstants.UI.ButtonTitle.OK); });
        }

        public static void AlertDuplicatePlatform(List<GamePlatform> gamePlatforms)
        {
            SwEditorLogger.LogWarning(SwEditorConstants.UI.DUPLICATE_ITEMS.Format(gamePlatforms));

            AlertError(SwEditorConstants.UI.DUPLICATE_ITEMS.Format(gamePlatforms), (int)SwErrors.ESettings.DuplicatePlatform, SwEditorConstants.UI.ButtonTitle.CLOSE);
        }

        public static void AlertSuccess(string selectedTitleName)
        {
            SwEditorUtils.CallOnNextUpdate(() => { Alert(SwEditorConstants.UI.NEW_CREDENTIALS_IDS_IN_SETTINGS.Format(selectedTitleName), SwEditorConstants.UI.ButtonTitle.AWESOME); });
        }

        public static void AlertTitleNotFound(List<TitleDetails> current)
        {
            SwEditorUtils.CallOnNextUpdate(() =>
            {
                if (Alert(SwEditorConstants.UI.CHOOSE_TITLE_MANUALLY, SwEditorConstants.UI.ButtonTitle.GO_TO_SETTINGS))
                    SwEditorUtils.OpenSettings();
            });
        }

        public static void AlertInvalidToken()
        {
            AlertError(SwEditorConstants.UI.INVALID_TOKEN, (int)SwErrors.ESettings.InvalidToken, SwEditorConstants.UI.ButtonTitle.OK);
        }

        public static void AlertConfigurationIsUpToDate()
        {
            Alert(SwEditorConstants.UI.CONFIGURATION_IS_UP_TO_DATE, SwEditorConstants.UI.ButtonTitle.OK);
        }

        public static void AlertMissingUnityPackage()
        {
            Alert(SwEditorConstants.UI.MISSING_UNITY_IAP, SwEditorConstants.UI.ButtonTitle.OK);
        }

        public static void TitleSavedToSettingsSuccess()
        {
            Alert(SwEditorConstants.UI.CONFIGURATION_SUCCESSFULLY_COPIED, SwEditorConstants.UI.ButtonTitle.OK);
        }

        #endregion


        #region --- Menu ---

        public static void AlertApplyBackupRules ()
        {
            EditorUtility.DisplayDialog(SwEditorConstants.UI.SUPERSONIC_WISDOM_SDK, SwEditorConstants.UI.APPLIED_BACKUP_RULES, SwEditorConstants.UI.ButtonTitle.OK);
        }

        public static void AlertApplyNetworkDebug()
        {
            EditorUtility.DisplayDialog(SwEditorConstants.UI.SUPERSONIC_WISDOM_SDK, SwEditorConstants.UI.APPLIED_NETWORK_DEBUG_CONFIGURATION, SwEditorConstants.UI.ButtonTitle.OK);
        }
        
        public static void AlertCannotApplyNetworkDebug()
        {
            AlertError(SwEditorConstants.UI.CANNOT_APPLY_NETWORK_DEBUG_CONFIGURATION, (int)SwErrors.EMenu.CannotApplyNetworkDebug, SwEditorConstants.UI.ButtonTitle.OK);
        }

        public static void AlertCannotApplyBackupRules ()
        {
            AlertError(SwEditorConstants.UI.CANNOT_APPLY_BACKUP_RULES, (int)SwErrors.EMenu.CannotApplyBackupRules, SwEditorConstants.UI.ButtonTitle.OK);
        }

        #endregion
    }
}