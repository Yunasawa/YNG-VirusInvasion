#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using Facebook.Unity.Editor;
using Facebook.Unity.Settings;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwGeneralStage10SettingsTab : SwGeneralCoreSettingsTab
    {
        #region --- Members ---

        private readonly GUIContent _facebookAppIdLabel = new GUIContent("Facebook SDK App ID", "Your Facebook App ID");
        private readonly GUIContent _facebookClientTokenLabel = new GUIContent("Facebook SDK Client Token", "Your Facebook Client Token");
        private readonly SwStage10ExternalParamsTabs _paramsWindow;
        
        #endregion


        #region --- Construction ---

        public SwGeneralStage10SettingsTab(SerializedObject soSettings, SwStage10ExternalParamsTabs parmaConfiguratorWindow) : base(soSettings)
        {
            _paramsWindow = parmaConfiguratorWindow;
        }

        #endregion


        protected override void OnTitleSelected()
        {
            base.OnTitleSelected();
            
            SwStage10EditorUtils.SetupGameAnalytics();
            SwSelfUpdate.CheckForUpdates();
        }


        #region --- Private Methods ---

        private static void HandleFacebookChange ()
        {
            EditorUtility.SetDirty(FacebookSettings.NullableInstance);
            ManifestMod.GenerateManifest();
        }

        protected internal override void DrawBody()
        {
            base.DrawBody();

            DrawFacebookAppIdField();
            GUILayout.Space(SPACE_BETWEEN_FIELDS);
            DrawFacebookClientTokenField();
            GUILayout.Space(SPACE_BETWEEN_FIELDS);
            DrawTimeBased();

            DrawResourceManagement();
            DrawParamsConfiguratorWindow();
        }

        private void DrawFacebookAppIdField ()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(_facebookAppIdLabel, GUILayout.Width(LABEL_WIDTH));
            var beforeAppId = SwEditorUtils.FacebookAppId;
            SwEditorUtils.FacebookAppId = TextFieldWithoutSpaces(beforeAppId);
            GUILayout.EndHorizontal();

            if (!SwEditorUtils.FacebookAppId.Equals(beforeAppId))
            {
                EditorUtility.SetDirty(FacebookSettings.Instance);
                HandleFacebookChange();
            }
        }

        private void DrawFacebookClientTokenField()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(_facebookClientTokenLabel, GUILayout.Width(LABEL_WIDTH));
            var beforeClientToken = SwEditorUtils.FacebookClientToken;
            SwEditorUtils.FacebookClientToken = TextFieldWithoutSpaces(beforeClientToken);
            GUILayout.EndHorizontal();
            
            if (!SwEditorUtils.FacebookClientToken.Equals(beforeClientToken))
            {
                EditorUtility.SetDirty(FacebookSettings.Instance);
                HandleFacebookChange();
            }
        }

        private void DrawTimeBased ()
        {
            GUILayout.BeginHorizontal();
            Settings.isTimeBased = GUILayout.Toggle(Settings.isTimeBased, "  Time Based Game");
            GUILayout.EndHorizontal();
        }

        private void DrawResourceManagement()
        {
            GUILayout.Space(SPACE_BETWEEN_FIELDS);
            Settings.enableResourceManagement = GUILayout.Toggle(Settings.enableResourceManagement, "  Enable Resource Management");
            
        }
		
		protected virtual void DrawParamsConfiguratorWindow()
        {
            if (!Settings.enableResourceManagement || _paramsWindow == null) return;

            DrawHorizontalLine();
            GUILayout.Space(SPACE_BETWEEN_FIELDS);
            _paramsWindow.Draw();
        }

        #endregion
    }
}
#endif