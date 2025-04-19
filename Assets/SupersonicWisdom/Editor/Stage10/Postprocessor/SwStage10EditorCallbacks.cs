#if SW_STAGE_STAGE10_OR_ABOVE

namespace SupersonicWisdomSDK.Editor
{
    internal class SwStage10EditorCallbacks : SwEditorCallbacks
    {
        #region --- Private Methods ---

        private static string[] OnWillSaveAssets(string[] paths)
        {
            SwResourceManagementEditorUtils.TryGenerateEnum();

            return paths;
        }
        
        #endregion
    }
}
#endif