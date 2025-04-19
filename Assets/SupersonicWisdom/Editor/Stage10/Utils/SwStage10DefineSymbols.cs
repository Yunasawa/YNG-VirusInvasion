#if SW_STAGE_STAGE10_OR_ABOVE
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwStage10DefineSymbols : SwDefineSymbols
    {
        #region --- Constants ---

        private const string RESOURCE_MANAGEMENT_ENABLED = "SW_RESOURCE_MANAGEMENT_ENABLED";

        #endregion


        #region --- Public Methods ---

        public override void UpdateRequiredSymbols()
        {
            base.UpdateRequiredSymbols();

            if (SwEditorUtils.SwSettings == null) return;

            SwEditorUtils.UpdateDefines(RESOURCE_MANAGEMENT_ENABLED, SwEditorUtils.SwSettings.enableResourceManagement, BuildTargetGroup.Android, BuildTargetGroup.iOS);
            SwInfra.Logger.Log(EWisdomLogType.Resource, $"Set define symbol ({RESOURCE_MANAGEMENT_ENABLED}) to {SwEditorUtils.SwSettings.enableResourceManagement}");
        }

        #endregion
    }
}
#endif