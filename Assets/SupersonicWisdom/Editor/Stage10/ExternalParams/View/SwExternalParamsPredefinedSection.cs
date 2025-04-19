#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal sealed class SwExternalParamsPredefinedSection : SwExternalParamsSection
    {
        #region --- Members ---

        private readonly Func<string, bool> _isAllowed;

        #endregion


        #region --- Properties ---

        private readonly List<SwPredefinedExternalParams> Params;

        #endregion


        #region --- Construction ---

        internal SwExternalParamsPredefinedSection(List<SwPredefinedExternalParams> parameters, GUIStyle regularBoxStyle, GUIStyle errorBoxStyle, int extraSpaceUnits, Func<string, bool> isAllowed) : base(regularBoxStyle, errorBoxStyle, extraSpaceUnits)
        {
            Params = parameters;
            _isAllowed = isAllowed;
        }

        #endregion


        #region --- Public Methods ---

        public void Draw(List<SwParamError> errors)
        {
            foreach (var key in Params)
            {
                var isError = SetErrors(errors, key);
                DrawKey(isError, key);
            }
        }

        #endregion


        #region --- Private Methods ---

        private void DrawKey(bool isError, SwPredefinedExternalParams parameter)
        {
            var isAllowed = _isAllowed?.Invoke(parameter.Key) ?? false;
            var style = isError ? _errorStyle : _regularStyle;
            
            using (new SwGUIVerticalScope(style, _extraSpaceUnits))
            {
                parameter.OnGui(false, isAllowed);
            }
        }

        private bool SetErrors(List<SwParamError> errors, SwBaseExternalParams key)
        {
            var error = IsKeyInError(key, errors);
            var isError = error != ESwExternalParamsParamError.None;

            if (isError)
            {
                key.SetErrors(error);
            }
            else
            {
                key.ClearErrors();
            }

            return isError;
        }

        #endregion
    }
}
#endif