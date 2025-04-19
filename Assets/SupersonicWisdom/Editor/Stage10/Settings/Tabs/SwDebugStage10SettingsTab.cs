#if SW_STAGE_STAGE10_OR_ABOVE

using UnityEditor;

#if SUPERSONIC_WISDOM
using System;
using UnityEngine;
#endif

namespace SupersonicWisdomSDK.Editor
{
    internal class SwDebugStage10SettingsTab : SwDebugCoreSettingsTab
    {
        #region --- Construction ---

        public SwDebugStage10SettingsTab(SerializedObject soSettings) : base(soSettings) { }

        #endregion


        #region --- Private Methods ---

        protected internal override void DrawContent()
        {
            base.DrawContent();
            
#if SUPERSONIC_WISDOM
            DrawInGameConsoleLog();
#endif
        }

#if SUPERSONIC_WISDOM
        private void DrawInGameConsoleLog()
        {
            GUILayout.Space(SPACE_BETWEEN_FIELDS);
            
            var enableInGameConsole = GUILayout.Toggle(Settings.inGameConsoleSettings.isEnable, "  Enable InGame Console Integration");
            
            Settings.inGameConsoleSettings.isEnable = enableInGameConsole;

            if (Settings.inGameConsoleSettings.isEnable)
            {
                GUILayout.Space(SPACE_BETWEEN_FIELDS);
                
                string[] options = { "Warning", "Error", "Exception" };
                var currentIndex = Array.IndexOf(options, Settings.inGameConsoleSettings.logDebugLevel);
                
                if (currentIndex == -1) currentIndex = 0;

                var selectedIndex = EditorGUILayout.Popup("Log Debug Level", currentIndex, options);
                
                Settings.inGameConsoleSettings.logDebugLevel = options[selectedIndex];
            }
        }
#endif
        #endregion
    }
}
#endif