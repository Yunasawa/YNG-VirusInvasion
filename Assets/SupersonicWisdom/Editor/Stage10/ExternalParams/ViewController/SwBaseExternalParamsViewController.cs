#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal abstract class SwBaseExternalParamsViewController
    {
        #region --- Constants ---

        internal const int EXTRA_SPACE_UNITS = 10;

        #endregion


        #region --- Members ---

        protected readonly GUIStyle _regularBoxStyle = SwResourceManagementEditorUtils.GetRegularBoxStyle(SwColors.SECTION);
        protected readonly GUIStyle _errorBoxStyle = SwResourceManagementEditorUtils.GetRegularBoxStyle(SwColors.ERROR);

        #endregion


        #region --- Public Methods ---

        protected abstract List<SwParamError> GetErrors();

        public void Draw()
        {
            Draw(GetErrors());
        }

        #endregion


        #region --- Private Methods ---

        protected abstract void Draw(List<SwParamError> errors);

        protected List<SwParamError> GetCollectionErrors(IEnumerable<SwBaseExternalParams> keysWrappers)
        {
            var errors = new List<SwParamError>();

            if (keysWrappers == null)
            {
                return errors;
            }

            foreach (var keysWrapper in keysWrappers)
            {
                if(!keysWrapper.IsEnabled) continue;
                
                var error = keysWrapper.Validate();

                if (error != ESwExternalParamsParamError.None)
                {
                    errors.Add(new SwParamError(keysWrapper, error));
                }
            }

            return errors;
        }
        
        protected IEnumerable<SwParamError> GetDuplicateNameKeysErrors(params SwBaseExternalParams[] paramsList)
        {
            var errors = new List<SwParamError>();

            if (paramsList.SwIsNullOrEmpty())
            {
                return errors;
            }

            var errorsSet = new HashSet<string>();

            foreach (var parameter in paramsList)
            {
                if (!parameter.IsEnabled) continue;
                
                if (errorsSet.Contains(parameter.Name))
                {
                    errors.Add(new SwParamError(parameter, ESwExternalParamsParamError.DuplicateName));
                }
                else
                {
                    errorsSet.Add(parameter.Name);
                }
            }

            return errors;
        }

        #endregion
    }
}
#endif