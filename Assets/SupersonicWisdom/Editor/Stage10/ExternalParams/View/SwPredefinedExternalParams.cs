#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    [Serializable]
    internal class SwPredefinedExternalParams : SwBaseExternalParams
    {
        #region --- Constants ---

        private const string IS_ENABLED_LABEL_TEXT = "Enabled";

        #endregion


        #region --- Events ---

        public event Action<string, bool> IsEnabledChangedEvent;

        #endregion


        #region --- Members ---

        private readonly string[] _options;

        private int _selectedIndex;

        #endregion


        #region --- Properties ---

        private bool IsEnabled
        {
            get { return _data.IsEnabled; }
            set { _data.IsEnabled = value; }
        }

        #endregion


        #region --- Construction ---

        public SwPredefinedExternalParams(SwExternalParamsData config, string[] options) : base(config)
        {
            Key = config.key;
            _options = options;
            _selectedIndex = Array.IndexOf(options, Key);
        }

        #endregion


        #region --- Public Methods ---

        public override ESwExternalParamsParamError Validate()
        {
            return !IsEnabled ? ESwExternalParamsParamError.None : base.Validate();
        }

        #endregion


        #region --- Private Methods ---

        public override void OnGui(bool canChangeKeyName, bool isAllowed)
        {
            using (new SwGUIEnabledScope(isAllowed))
            {
                DrawIsEnabled();
            }

            using (new SwGUIEnabledScope(isAllowed && IsEnabled))
            {
                base.OnGui(canChangeKeyName, false);
            }
        }

        protected override void DrawKey(bool canChangeKeyName)
        {
            using (new SwGUIEnabledScope(canChangeKeyName))
            {
                _selectedIndex = EditorGUILayout.Popup(PARAMETER_KEY_LABEL_TEXT, _selectedIndex, _options);
                Key = _options[_selectedIndex];
            }
        }

        private void DrawIsEnabled()
        {
            GUILayout.BeginHorizontal();
            var previousIsEnabled = IsEnabled;
            var nextIsEnabled = EditorGUILayout.Toggle(IS_ENABLED_LABEL_TEXT, IsEnabled);

            if (previousIsEnabled != nextIsEnabled)
            {
                IsEnabled = nextIsEnabled;
                IsEnabledChangedEvent?.Invoke(Key, IsEnabled);
            }

            GUILayout.EndHorizontal();
        }

        #endregion
    }
}
#endif