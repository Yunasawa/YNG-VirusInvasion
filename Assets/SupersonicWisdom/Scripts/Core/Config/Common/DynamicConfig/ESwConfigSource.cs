namespace SupersonicWisdomSDK
{
    /// <summary>
    /// This enum represents the source of the configuration.
    /// A lower value means a higher priority.
    /// (For example if two keys have the same value, the one with the lower priority will be used)
    /// </summary>
    public enum ESwConfigSource
    {
        WisdomSettings = 6,
        WisdomLocal = 5,
        WisdomRemote = 4,
        UgsRemote = 3,
        WisdomAb = 2,
        WisdomDeeplink = 1,
    }
}