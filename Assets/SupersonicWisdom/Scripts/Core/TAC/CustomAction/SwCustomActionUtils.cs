#if SUPERSONIC_WISDOM_PARTNER
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal static class SwCustomActionUtils
    {
        #region --- Members ---
        
        private static readonly Dictionary<ESwCustomActionTrigger, string> TriggerMappings = new Dictionary<ESwCustomActionTrigger, string>
        {
            { ESwCustomActionTrigger.LevelStarted, "sw_level_started" },
            { ESwCustomActionTrigger.LevelCompleted, "sw_level_completed" },
            { ESwCustomActionTrigger.Playtime, "sw_playtime_tick" },
        };
        
        #endregion
        
        
        #region --- Public Methods ---
        
        internal static SwActionData ConvertToSwActionData(SwCustomAction customActionData)
        {
            var action = new SwActionData
            {
                Id = ConvertActionNameToId(customActionData.ActionName),
                Priority = customActionData.Priority,
                ActionType = ESwActionType.Custom,
                SearchType = ESearchType.OR,
                Triggers = ConvertCustomActionTriggers(customActionData.Triggers),
                Conditions = ConvertCustomTriggerConditions(customActionData.Conditions),
            };

            return action;
        }
        
        #endregion
        
        
        #region --- Private Methods ---
        
        private static string ConvertActionNameToId(string actionName)
        {
            return $"Show-{actionName.ToLower()}";
        }
        
        private static string[] ConvertCustomActionTriggers(ESwCustomActionTrigger[] triggers)
        {
            var triggerList = new List<string>();
            
            foreach (var trigger in triggers)
            {
                if (TriggerMappings.TryGetValue(trigger, out var mapping))
                {
                    triggerList.Add(mapping);
                }
            }

            return triggerList.ToArray();
        }
        
        private static SwCoreTacConditions ConvertCustomTriggerConditions(SwCustomTriggerConditions conditions)
        {
            var swConditions = new SwCoreTacConditions();
            
            if (conditions == null)
            {
                return swConditions;
            }
            
            if (conditions.ExactLevels != null)
            {
                swConditions.ExactLevels = conditions.ExactLevels;
            }
            
            if (conditions.MinLevel != null)
            {
                swConditions.MinLevel = conditions.MinLevel;
            }
            
            if (conditions.MaxLevel != null)
            {
                swConditions.MaxLevel = conditions.MaxLevel;
            }
            
            if (conditions.MinSecond != null)
            {
                swConditions.MinSecond = conditions.MinSecond;
            }
            
            if (conditions.MaxSecond != null)
            {
                swConditions.MaxSecond = conditions.MaxSecond;
            }
            
            if (conditions.ConsecutiveCompletedLevels != null)
            {
                swConditions.ConsecutiveCompletedLevels = conditions.ConsecutiveCompletedLevels;
            }
            
            if (conditions.ConsecutiveFailedLevels != null)
            {
                swConditions.ConsecutiveFailedLevels = conditions.ConsecutiveFailedLevels;
            }
            
            if (conditions.MinLevelAttempts != null)
            {
                swConditions.MinLevelAttempts = conditions.MinLevelAttempts;
            }
            
            if (conditions.MinLevelRevives != null)
            {
                swConditions.MinLevelRevives = conditions.MinLevelRevives;
            }
            
            if (conditions.MinCompletedLevels != null)
            {
                swConditions.MinCompletedLevels = conditions.MinCompletedLevels;
            }
            
            if (conditions.MinCompletedBonusLevels != null)
            {
                swConditions.MinCompletedBonusLevels = conditions.MinCompletedBonusLevels;
            }
            
            if (conditions.MinCompletedTutorialLevels != null)
            {
                swConditions.MinCompletedTutorialLevels = conditions.MinCompletedTutorialLevels;
            }
            
            if (conditions.IsNoAds != null)
            {
                swConditions.IsNoAds = conditions.IsNoAds;
            }
            
            return swConditions;
        }
        
        #endregion
    }
}
#endif