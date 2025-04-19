using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    public static class SwTacUtils
    {
        internal static string ModifyInternalTrigger(string trigger)
        {
            return SwTacSystem.INTERNAL_TRIGGER_FORMAT.Format(trigger.ToLower());
        }

        internal static void LogNewTriggerReached(string triggerReached)
        {
            SwInfra.Logger.Log(EWisdomLogType.TAC, "New triggers reached - {0}".Format(triggerReached));
        }

        internal static void LogAction(ESwActionType actionToPerform)
        {
            if (actionToPerform == ESwActionType.None) return;
            
            SwInfra.Logger.Log(EWisdomLogType.TAC, "Trying to perform action - {0}".Format(actionToPerform.SwToString()));
        }
        
        internal static void ParseTriggers(SwActionData action, KeyValuePair<string, object> overridePair)
        {
            action.Triggers = SwUtils.JsonHandler.DeserializeObject<string[]>(overridePair.Value.ToString());
        }
        
        internal static void ValidateListeners(List<ISwTacSystemListener> listeners)
        {
            var listenerTypes = new HashSet<ESwActionType>();

            foreach (var listener in listeners)
            {
                if(listener.ActionType == ESwActionType.Custom && listenerTypes.Contains(ESwActionType.Custom)) continue;
                
                if (listenerTypes.Contains(listener.ActionType))
                {
                    SwInfra.Logger.LogError(EWisdomLogType.TAC, $"We have two listeners responding to the same ActionType : {listener.ActionType}");
                    Application.Quit();
                }

                listenerTypes.Add(listener.ActionType);
            }
        }

        internal static void ModifyConditionsOnActionPerformed(SwActionData performedAction, bool isTimeBased)
        {
            if (performedAction.Conditions == null) return;

            var listSingleUseAboveSeconds = performedAction.Conditions.SingleUseAboveSeconds?.ToList();

            if (isTimeBased && !listSingleUseAboveSeconds.SwIsNullOrEmpty())
            {
                listSingleUseAboveSeconds?.RemoveAt(0);
                performedAction.Conditions.SingleUseAboveSeconds = listSingleUseAboveSeconds?.ToArray();
            }
        }

        internal static bool CheckCondition(SwCoreTacConditions conditions, SwCoreUserData _userData, EConfigStatus _configStatus, SwGameStateSystem _gameStateSystem, SwTimerManager _timerManager, HashSet<ESwActionType> CurrentIterationActionsSet)
        {
            if (conditions == null)
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, "Conditions are null. Returning true.");

                return true;
            }

            var userState = _userData?.ImmutableUserState();

            if (userState == null)
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, "User state is null. Returning false.");

                return false;
            }

            if (conditions.IsDuringLevel.HasValue && conditions.IsDuringLevel.Value == userState.isDuringLevel)
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: IsDuringLevel is {conditions.IsDuringLevel.Value} but userState.isDuringLevel is {userState.isDuringLevel}");

                return false;
            }

            if (!conditions.ConfigStatus.SwIsNullOrEmpty() && !conditions.ConfigStatus.Contains(_configStatus))
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: ConfigStatus doesn't contain {_configStatus}");

                return false;
            }

            if (!conditions.CurrentState.SwIsNullOrEmpty() && !conditions.CurrentState.Contains(_gameStateSystem.CurrentGameState))
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: CurrentState doesn't contain {_gameStateSystem.CurrentGameState}");

                return false;
            }

            if (!conditions.PreviousState.SwIsNullOrEmpty() && !conditions.PreviousState.Contains(_gameStateSystem.PreviousGameState))
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: PreviousState doesn't contain {_gameStateSystem.PreviousGameState}");

                return false;
            }

            if (conditions.MinAge.HasValue && conditions.MinAge.Value > userState.age)
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: MinAge is {conditions.MinAge.Value} but userState.age is {userState.age}");

                return false;
            }

            if (conditions.ConsecutiveCompletedLevels.HasValue && conditions.ConsecutiveCompletedLevels.Value > userState.consecutiveCompletedLevels)
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: ConsecutiveCompletedLevels is {conditions.ConsecutiveCompletedLevels.Value} but userState.consecutiveCompletedLevels is {userState.consecutiveCompletedLevels}");

                return false;
            }

            if (conditions.ConsecutiveFailedLevels.HasValue && conditions.ConsecutiveFailedLevels.Value > userState.consecutiveFailedLevels)
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: ConsecutiveFailedLevels is {conditions.ConsecutiveFailedLevels.Value} but userState.consecutiveFailedLevels is {userState.consecutiveFailedLevels}");

                return false;
            }

            if (conditions.MinLevelAttempts.HasValue && conditions.MinLevelAttempts.Value > userState.levelAttempts)
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: MinLevelAttempts is {conditions.MinLevelAttempts.Value} but userState.levelAttempts is {userState.levelAttempts}");

                return false;
            }

            if (conditions.MinLevelRevives.HasValue && conditions.MinLevelRevives.Value > userState.levelRevives)
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: MinLevelRevives is {conditions.MinLevelRevives.Value} but userState.levelRevives is {userState.levelRevives}");

                return false;
            }

            if (conditions.MinCompletedLevels.HasValue && conditions.MinCompletedLevels.Value > userState.completedLevels)
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: MinCompletedLevels is {conditions.MinCompletedLevels.Value} but userState.completedLevels is {userState.completedLevels}");

                return false;
            }

            if (conditions.MinCompletedBonusLevels.HasValue && conditions.MinCompletedBonusLevels.Value > userState.completedBonusLevels)
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: MinCompletedBonusLevels is {conditions.MinCompletedBonusLevels.Value} but userState.completedBonusLevels is {userState.completedBonusLevels}");

                return false;
            }

            if (conditions.MinCompletedTutorialLevels.HasValue && conditions.MinCompletedTutorialLevels.Value > userState.completedTutorialLevels)
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: MinCompletedTutorialLevels is {conditions.MinCompletedTutorialLevels.Value} but userState.completedTutorialLevels is {userState.completedTutorialLevels}");

                return false;
            }

            if (conditions.MaxLevel.HasValue && conditions.MaxLevel.Value < userState.lastLevelStarted)
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: MaxLevel is {conditions.MaxLevel.Value} but userState.lastLevelStarted is {userState.lastLevelStarted}");

                return false;
            }

            if (conditions.MinLevel.HasValue && conditions.MinLevel.Value > userState.lastLevelStarted)
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: MinLevel is {conditions.MinLevel.Value} but userState.lastLevelStarted is {userState.lastLevelStarted}");

                return false;
            }

            if (!conditions.ExactLevels.SwIsNullOrEmpty() && !conditions.ExactLevels.ToList().Contains((int)userState.lastLevelStarted))
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: ExactLevels does not contain {userState.lastLevelStarted}");

                return false;
            }

            if (conditions.MaxSecond.HasValue && conditions.MaxSecond.Value < _timerManager.AllSessionsPlaytimeNeto)
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: MaxSecond is {conditions.MaxSecond.Value} but AllSessionsPlaytimeNeto is {_timerManager.AllSessionsPlaytimeNeto}");

                return false;
            }

            if (conditions.MinSecond.HasValue && conditions.MinSecond.Value > _timerManager.AllSessionsPlaytimeNeto)
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: MinSecond is {conditions.MinSecond.Value} but AllSessionsPlaytimeNeto is {_timerManager.AllSessionsPlaytimeNeto}");

                return false;
            }
            
            if (conditions.SingleUseAboveSeconds != null && (conditions.SingleUseAboveSeconds.Length == 0 || (conditions.SingleUseAboveSeconds.FirstOrDefault() == default ? int.MinValue : conditions.SingleUseAboveSeconds.First()) > _timerManager.AllSessionsPlaytimeNeto))
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: SingleUseAboveSeconds is lower than AllSessionsPlaytimeNeto {_timerManager.AllSessionsPlaytimeNeto}");

                return false;
            }

            if (!conditions.AllowAfter.SwIsNullOrEmpty() && CurrentIterationActionsSet.Count != 0 && !CurrentIterationActionsSet.Intersect(conditions.AllowAfter).Any())
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, "Condition failed: No intersection between AllowAfter and CurrentIterationActionsSet.");

                return false;
            }

            return true;
        }
    }
}