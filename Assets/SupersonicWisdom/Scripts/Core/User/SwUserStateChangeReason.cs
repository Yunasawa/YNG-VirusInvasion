namespace SupersonicWisdomSDK
{
    internal enum SwUserStateChangeReason
    {
        None = -1,
        GameStart,
        SessionStart,
        UpdateConversionValue,
        UpdateImpConversionValue,
        LevelCompleted,
        LevelSkipped,
        LevelRevived,
        LevelFailed,
        Revenue,
        PayingUser,
        NoAdsStateChange,
    }
}