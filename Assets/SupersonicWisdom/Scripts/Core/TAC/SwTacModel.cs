using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    internal class SwTacModel
    {
        #region --- Members ---

        private Dictionary<ESwActionType, List<SwActionData>> _actionsByActionTypeDict;
        private Dictionary<ESwActionType, List<SwActionData>> _actionsAfterActionDict;

        #endregion

        
        #region --- Properties ---

        public int MaximumUiActions { get; private set; }
        public Dictionary<string, List<SwActionData>> ActionsByTriggerDict { get; private set; }

        #endregion

        
        #region --- Construction ---

        public SwTacModel(SwTacConfig triggerData, float allSessionsPlaytimeNeto)
        {
            SwInfra.Logger.Log(EWisdomLogType.TAC, SwUtils.JsonHandler.SerializeObject(triggerData));
            
            triggerData.Actions = EditTriggerData(triggerData, allSessionsPlaytimeNeto);
            
            InitDataStructures();
            UpdateTriggersData(triggerData);
        }

        private static SwActionData[] EditTriggerData(SwTacConfig triggerData, float allSessionsPlaytimeNeto)
        {
            var sortedActionData = triggerData.Actions.OrderBy(t => -t.Priority).ToArray();

            foreach (var actionData in sortedActionData)
            {
                var singleUseAboveSeconds = actionData?.Conditions?.SingleUseAboveSeconds?.ToList();

                if (singleUseAboveSeconds == null)
                {
                    continue;
                }
                
                singleUseAboveSeconds.RemoveAll(value => value <= allSessionsPlaytimeNeto);

                actionData.Conditions.SingleUseAboveSeconds = singleUseAboveSeconds.ToArray();
            }

            return sortedActionData;
        }

        #endregion


        #region --- Public Methods ---

        public List<SwActionData> GetActionsByTrigger(string keyTrigger)
        {
            return keyTrigger.SwIsNullOrEmpty() || !ActionsByTriggerDict.TryGetValue(keyTrigger, out var value) ? null : value;
        }

        public List<SwActionData> GetActionAfterAction(string currentTrigger, params ESwActionType[] performedActions)
        {
            var actionsAllowed = ActionsByTriggerDict.SwSafelyGet(currentTrigger, new List<SwActionData>());

            foreach (var performedAction in performedActions)
            {
                var actionAfterAction = _actionsAfterActionDict.SwSafelyGet(performedAction, new List<SwActionData>());
                actionsAllowed = actionsAllowed.Intersect(actionAfterAction).ToList();
            }

            return actionsAllowed;
        }

        public void UpdateActions(SwActionData action)
        {
            if (action == null) return;

            AddOrUpdateActions(ActionsByTriggerDict, action, action.Triggers);
            AddOrUpdateActions(_actionsAfterActionDict, action, action.Conditions?.AllowAfter);
            AddOrUpdateActionByType(action);
        }

        #endregion

        
        #region --- Private Methods ---

        private void InitDataStructures()
        {
            ActionsByTriggerDict = new Dictionary<string, List<SwActionData>>();
            _actionsAfterActionDict = new Dictionary<ESwActionType, List<SwActionData>>();
            _actionsByActionTypeDict = new Dictionary<ESwActionType, List<SwActionData>>();
        }

        private void UpdateTriggersData([NotNull] SwTacConfig tacConfig)
        {
            ActionsByTriggerDict.Clear();
            _actionsAfterActionDict.Clear();
            _actionsByActionTypeDict.Clear();

            MaximumUiActions = tacConfig.MaximumUiActions;

            var actionsData = tacConfig.Actions;

            if (actionsData == null || !actionsData.Any()) return;

            // Create empty list for each action type
            foreach (var type in Enum.GetValues(typeof(ESwActionType)).Cast<ESwActionType>())
            {
                if (!_actionsAfterActionDict.ContainsKey(type))
                {
                    _actionsAfterActionDict.Add(type, new List<SwActionData>());
                }

                if (!_actionsByActionTypeDict.ContainsKey(type))
                {
                    _actionsByActionTypeDict.Add(type, new List<SwActionData>());
                }
            }

            // Fill both lists
            foreach (var actionData in actionsData.Where(ad => !ad.Triggers.SwIsNullOrEmpty()))
            {
                foreach (var keyTrigger in actionData.Triggers)
                {
                    if (!ActionsByTriggerDict.ContainsKey(keyTrigger))
                    {
                        ActionsByTriggerDict.Add(keyTrigger, new List<SwActionData>());
                    }

                    ActionsByTriggerDict[keyTrigger].Add(actionData);
                    _actionsByActionTypeDict[actionData.ActionType].Add(actionData);
                }

                if (actionData.Conditions == null || actionData.Conditions.AllowAfter.SwIsNullOrEmpty())
                {
                    continue;
                }

                foreach (var allowAfter in actionData.Conditions.AllowAfter)
                {
                    _actionsAfterActionDict[allowAfter].Add(actionData);
                }
            }
        }

        private static void AddOrUpdateActions<T>(Dictionary<T, List<SwActionData>> actionDict, SwActionData action, IEnumerable<T> keys)
        {
            if (keys == null) return;

            foreach (var key in keys)
            {
                if (!actionDict.TryGetValue(key, out var actions))
                {
                    actions = new List<SwActionData>();
                    actionDict[key] = actions;
                }

                var existingAction = actions.FirstOrDefault(a => a.Id == action.Id);
                
                if (existingAction != null)
                {
                    actions.Remove(existingAction);
                }

                actions.Add(action);
            }
        }

        private void AddOrUpdateActionByType(SwActionData action)
        {
            if (!_actionsByActionTypeDict.TryGetValue(action.ActionType, out var actionTypeList))
            {
                actionTypeList = new List<SwActionData>();
                _actionsByActionTypeDict[action.ActionType] = actionTypeList;
            }

            var existingActionByType = actionTypeList.FirstOrDefault(a => a.Id == action.Id);
            
            if (existingActionByType != null)
            {
                actionTypeList.Remove(existingActionByType);
            }

            actionTypeList.Add(action);
        }

        #endregion
    }
}