namespace SupersonicWisdomSDK
{
    internal static class SwTrackerConstants
    {
        #region --- Constants ---

        // https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
        internal const string FLOAT_POINT_6 = "F6";
        
        #endregion
        
        
        #region --- Inner Enumerations ---

        internal enum EMinimumSdkVersion
        {
            // ReSharper disable once InconsistentNaming
            Sdk_7_7_18 = 7071899,
            Sdk_8_1_0 = 8010099,
        }
        
        #endregion
        
        
        #region --- Public Methods ---
        
        public static bool ShouldTrackPastMinimumSdkVersion(SwCoreUserData coreUserData, EMinimumSdkVersion minimumSdkVersion)
        {
            // In use due to - https://supersonicstudio.monday.com/boards/883112163/pulses/6277235145
            // And - https://supersonicstudio.monday.com/boards/883112163/views/146103280/pulses/6975217843
            return SwUtils.System.IsSdkDevelopmentBuild || coreUserData.InstallSdkVersionId >= (long)minimumSdkVersion;
        }
        
        #endregion
    }
}