using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    public class SwCustomTriggerConditions
    {
        [JsonProperty("consecutiveCompletedLevels")] public int? ConsecutiveCompletedLevels { get; set; }
        [JsonProperty("consecutiveFailedLevels")] public int? ConsecutiveFailedLevels { get; set; }
        [JsonProperty("minLevel")] public int? MinLevel { get; set; }
        [JsonProperty("minSecond")] public int? MinSecond { get; set; }
        [JsonProperty("maxLevel")] public int? MaxLevel { get; set; }
        [JsonProperty("maxSecond")] public int? MaxSecond { get; set; }
        [JsonProperty("exactLevels")] public int[] ExactLevels { get; set; }
        [JsonProperty("minLevelAttempts")] public int? MinLevelAttempts { get; set; }
        [JsonProperty("minLevelRevives")] public int? MinLevelRevives { get; set; }
        [JsonProperty("minCompletedLevels")] public int? MinCompletedLevels { get; set; }
        [JsonProperty("minCompletedBonusLevels")] public int? MinCompletedBonusLevels { get; set; }
        [JsonProperty("minCompletedTutorialLevels")] public int? MinCompletedTutorialLevels { get; set; }
        [JsonProperty("isNoAds")] public bool? IsNoAds { get; set; }
    }
}