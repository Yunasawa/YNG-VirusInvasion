#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal abstract class SwExternalParamsSection
    {
        #region --- Members ---

        protected readonly int _extraSpaceUnits;
        protected readonly GUIStyle _errorStyle;
        protected readonly GUIStyle _regularStyle;

        #endregion


        #region --- Construction ---

        protected SwExternalParamsSection(GUIStyle regularStyle, GUIStyle errorStyle, int extraSpaceUnits)
        {
            _regularStyle = regularStyle;
            _errorStyle = errorStyle;
            _extraSpaceUnits = extraSpaceUnits;
        }

        #endregion


        #region --- Private Methods ---

        protected ESwExternalParamsParamError IsKeyInError(SwBaseExternalParams key, List<SwParamError> errors)
        {
            if (errors.SwIsNullOrEmpty())
            {
                return ESwExternalParamsParamError.None;
            }

            var error = errors.FirstOrDefault(error => error.Key == key);

            return error?.Error ?? ESwExternalParamsParamError.None;
        }

        #endregion
    }
}
#endif