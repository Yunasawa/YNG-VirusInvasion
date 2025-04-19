#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;

namespace SupersonicWisdomSDK.Editor
{
    internal abstract class SwPredefinedExternalParamsViewController : SwBaseExternalParamsViewController
    {
        #region --- Members ---

        protected readonly Dictionary<string, bool> _isEnabledDict = new Dictionary<string, bool>();

        #endregion


        #region --- Properties ---

        protected List<SwPredefinedExternalParams> Params { get; } = new List<SwPredefinedExternalParams>();

        #endregion


        #region --- Public Methods ---

        protected override List<SwParamError> GetErrors()
        {
            var allErrors = new List<SwParamError>();

            allErrors.AddRange(GetCollectionErrors(Params));
            allErrors.AddRange(GetDuplicateNameKeysErrors(Params.ToArray()));

            return allErrors;
        }

        #endregion


        #region --- Private Methods ---

        protected bool IsEnabled(string paramType)
        {
            return _isEnabledDict.SwSafelyGet(paramType, false);
        }

        #endregion
    }
}
#endif