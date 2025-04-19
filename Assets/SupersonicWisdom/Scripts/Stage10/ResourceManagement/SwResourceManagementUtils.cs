#if SW_STAGE_STAGE10_OR_ABOVE
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal static class SwResourceManagementUtils
    {
        #region --- Constants ---

        internal const string ASSET_PATH = "SupersonicWisdom/SwResourceManagmentKeysConfiguration";

        #endregion
        
        internal static SwResourcesManagementExternalParamsData Load()
        {
            return Resources.Load<SwResourcesManagementExternalParamsData>(ASSET_PATH);
        }
    }
}
#endif