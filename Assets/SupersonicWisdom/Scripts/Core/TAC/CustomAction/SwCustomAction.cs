#if SUPERSONIC_WISDOM_PARTNER
using System;

namespace SupersonicWisdomSDK
{
    /// <summary>
    /// This class is used to define the conditions that must be met for a custom action to be activated by the TAC system
    /// </summary>
    public class SwCustomAction
    {
        #region --- Properties ---
        
        /// <summary>
        /// Name of the action
        /// </summary>
        public string ActionName { get; }
        /// <summary>
        /// The priority of the action. Higher is first - 0 is invalid, must be between 1 and 100
        /// </summary>
        public int Priority { get; }
        /// <summary>
        /// The triggers that will activate the action
        /// </summary>
        public ESwCustomActionTrigger[] Triggers { get; }
        /// <summary>
        /// The conditions that must be met for the action to be activated
        /// </summary>
        public SwCustomTriggerConditions Conditions { get; }
        
        #endregion
        
        
        #region --- Construction ---
        
        public SwCustomAction(string actionName, int priority, ESwCustomActionTrigger[] triggers, SwCustomTriggerConditions conditions)
        {
            if (priority is < 1 or > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(priority), "Priority must be between 1 and 100");
            }
            
            ActionName = actionName;
            Priority = priority;
            Triggers = triggers;
            Conditions = conditions;
        }
        
        #endregion
    }
}
#endif