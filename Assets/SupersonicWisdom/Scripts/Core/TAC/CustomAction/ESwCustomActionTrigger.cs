#if SUPERSONIC_WISDOM_PARTNER
namespace SupersonicWisdomSDK
{
    public enum ESwCustomActionTrigger
    {
        LevelStarted, // Will be triggered when a level is started
        LevelCompleted, // Will be triggered when a level is completed
        Playtime, // Will be triggered in a fixed interval for time based games
    }
}
#endif