using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    internal class SwCoreTacConditions : SwCustomTriggerConditions
    {
        [JsonProperty("configStatus")] public EConfigStatus[] ConfigStatus { get; set; }
        [JsonProperty("isDuringLevel")] public bool? IsDuringLevel { get; set; }
        [JsonProperty("currentState")] public SwSystemState.EGameState[] CurrentState { get; set; }
        [JsonProperty("previousState")] public SwSystemState.EGameState[] PreviousState { get; set; }
        [JsonProperty("minAge")] public int? MinAge { get; set; }
        [JsonProperty("singleUseAboveSeconds")] public int[] SingleUseAboveSeconds { get; set; }
        [JsonProperty("allowAfter")] public ESwActionType[] AllowAfter { get; set; }
        [JsonProperty("minActiveDays")] public int? MinActiveDays { get; set; }
        [JsonProperty("minRevenue")] public float? MinRevenue { get; set; }
        [JsonProperty("freeZone")] public bool? AdFreeZone { get; set; }
        
        public override string ToString()
        {
            return SwUtils.JsonHandler.SerializeObject(this);
        }
    }
}