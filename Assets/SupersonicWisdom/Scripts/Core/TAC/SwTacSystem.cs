using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwTacSystem : ISwTacSystem
    {
        #region --- Constants ---
        
        internal const string METADATA_ACTION_ID_KEY_NAME = "actionId";
        internal const string INTERNAL_TRIGGER_FORMAT = "sw_{0}";
        internal const string TAC_MAXIMUM_UI_ACTIONS = "swTacMaximumUiActions";

        #endregion


        #region --- Members ---
        
        private bool _isEnabled;
        private bool _isIterating;
        private SwTacModel _model;
        protected SwSettings _settings;
        private SwCoreUserData _userData;
        private EConfigStatus _configStatus;
        private SwTimerManager _timerManager;
        private SwGameStateSystem _gameStateSystem;
        private List<ISwTacSystemListener> _listeners;
        private List<ESwActionType> _uiActionTypes = new List<ESwActionType>();
        private SwCustomTacListener _customTacListener;
        private SwTacConfig _configTriggerData;
        private SwTacConfig _combinedTriggerData;
        private List<SwActionData> _partnersActions;

        #endregion


        #region --- Properties ---
        internal SwTacModel Model
        {
            get { return _model; }
        }

        public HashSet<ESwActionType> CurrentIterationActionsSet { get; private set; }
       
        public HashSet<string> CurrentIterationActionsIdsSet { get; private set; }

        public Tuple<EConfigListenerType, EConfigListenerType> ListenerType
        {
            get { return new Tuple<EConfigListenerType, EConfigListenerType>(EConfigListenerType.Construction, EConfigListenerType.EndOfGame); }
        }

        private int UiActionsPerformedInCurrentIteration
        {
            get
            {
                return CurrentIterationActionsSet.Count(actionType => _uiActionTypes.Contains(actionType));
            }
        }

        #endregion
        

        #region --- Public Methods ---
        
        public void AddOrUpdateAction(SwActionData action, Func<IEnumerator> callback)
        {
            AddPartnerAction(action);

            _model = new SwTacModel(GetMergeConfigs(), _timerManager?.AllSessionsPlaytimeNeto ?? 0);
            
            HandleCustomTacListener(action, callback);
        }

        private void HandleCustomTacListener(SwActionData action, Func<IEnumerator> callback)
        {
            if (_customTacListener == null)
            {
                _customTacListener = new SwCustomTacListener();
                AddListeners(_customTacListener);
            }
            
            _customTacListener.AddOrUpdateAction(action, callback);
        }

        public void AddListeners(params ISwTacSystemListener[] listeners)
        {
            _listeners ??= new List<ISwTacSystemListener>();
            _listeners.AddRange(listeners);

            SwTacUtils.ValidateListeners(_listeners);
            SetUiActionTypes();
        }

        public IEnumerator FireTriggers(params string[] newTriggers)
        {
            yield return FireTriggers(metaData: null, newTriggers: newTriggers);
        }

        public IEnumerator FireTriggers(Dictionary<string, object> metaData, params string[] newTriggers)
        {
            if (!_isEnabled || newTriggers.SwIsNullOrEmpty()) yield break;

            SwInfra.Logger.Log(EWisdomLogType.TAC, $"Is iteration : {_isIterating}");
            
            yield return new WaitWhile(() => _isIterating);
            
            SwInfra.Logger.Log(EWisdomLogType.TAC, "Handling new triggers");
            
            CurrentIterationActionsSet.Clear();
            _isIterating = true;

            foreach (var newTrigger in newTriggers)
            {
                SwTacUtils.LogNewTriggerReached(newTrigger);
                
                var optionalsAction = _model.GetActionsByTrigger(newTrigger);

                yield return TryPerformPotentialActions(optionalsAction, newTrigger, metaData);
            }

            _isIterating = false;
        }

        public void InternalFireTriggers(params string[] triggers)
        {
            if (!_isEnabled || triggers.SwIsNullOrEmpty()) return;

            foreach (var trigger in triggers)
            {
                SwInfra.CoroutineService.StartCoroutine(FireTriggers(SwTacUtils.ModifyInternalTrigger(trigger)));
            }
        }

        public IEnumerator InternalFireTriggersRoutine(params string[] triggers)
        {
            if (!_isEnabled || triggers.SwIsNullOrEmpty()) yield break;

            foreach (var trigger in triggers)
            {
                yield return FireTriggers(SwTacUtils.ModifyInternalTrigger(trigger));
            }
        }

        public void OnConfigResolved(ISwCoreInternalConfig swCoreInternal, ISwConfigManagerState state, ISwConfigAccessor swConfigAccessor, ESwConfigSource source)
        {
            if (state != null)
            {
                _configStatus = state.Status;
            }

            var triggerData = SwTacCoreConfigOverrideUtils.OverrideTacConfig(swCoreInternal, swConfigAccessor);

            if (triggerData == null) return;
            
            _configTriggerData = triggerData;
            _model = new SwTacModel(GetMergeConfigs(), _timerManager?.AllSessionsPlaytimeNeto ?? 0);
            CurrentIterationActionsSet = new HashSet<ESwActionType>();
            CurrentIterationActionsIdsSet = new HashSet<string>();
            
            _isEnabled = true;
            SwInfra.Logger.Log(EWisdomLogType.TAC, $"Trigger Action System Enabled = {_isEnabled}");
        }

        #endregion


        #region --- Private Methods ---

        protected void InjectDependencies(SwCoreUserData userData, SwTimerManager timerManager, SwGameStateSystem gameStateSystem, SwSettings settings)
        {
            _userData = userData;
            _timerManager = timerManager;
            _gameStateSystem = gameStateSystem;
            _settings = settings;
        }

        protected virtual bool CheckCondition(SwCoreTacConditions conditions)
        {
            if (conditions == null) return true;

            var userState = _userData?.ImmutableUserState();

            if (userState == null) return false;

            return SwTacUtils.CheckCondition(conditions, _userData, _configStatus, _gameStateSystem, _timerManager, CurrentIterationActionsSet);
        }

        private IEnumerator TryPerformPotentialActions(List<SwActionData> optionalActions, string newTrigger, Dictionary<string, object> metaData)
        {
            if (optionalActions.SwIsNullOrEmpty())
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"No valid actions for {newTrigger}");
                yield break;
            }
            
            foreach (var optionalAction in optionalActions)
            {
                metaData ??= new Dictionary<string, object>();
                metaData[METADATA_ACTION_ID_KEY_NAME] = optionalAction.Id;
                
                if (optionalAction.ActionType == ESwActionType.None)
                {
                    SwInfra.Logger.LogError(EWisdomLogType.TAC, $"Action (id={optionalAction.Id}) has None action type");
                    continue;
                }
                
                var isConditionMet = IsConditionsMet(optionalAction, newTrigger);
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Action (id={optionalAction.Id}) condition is = {isConditionMet.ToString()}");
                
                if (!isConditionMet)
                {
                    continue;
                }

                SwTacUtils.LogAction(optionalAction.ActionType);

                if (_listeners == null) yield break;
                
                foreach (var listener in _listeners)
                {
                    if (listener == null || listener.ActionType != optionalAction.ActionType) continue;

                    listener.DidPerformAction = false;
                    SwInfra.Logger.Log(EWisdomLogType.TAC, $"Listener found ({listener}) for {listener.ActionType}");
                    
                    yield return listener.TryPerformAction(metaData);

                    SwInfra.Logger.Log(EWisdomLogType.TAC, $"Listener ({listener}) did perform = {listener.DidPerformAction}");
                    if (!listener.DidPerformAction) continue;

                    OnActionPerformed(optionalAction);

                    if (UiActionsPerformedInCurrentIteration >= _model.MaximumUiActions)
                    {
                        SwInfra.Logger.Log(EWisdomLogType.TAC, $"Reached Maximum UI Action {UiActionsPerformedInCurrentIteration} >= {_model.MaximumUiActions}");
                        yield break;
                    }
                    
                    var nextActions = _model.GetActionAfterAction(newTrigger, CurrentIterationActionsSet.ToArray());

                    // Since enumerator can't change during iteration, we use recursive and BREAK.
                    yield return TryPerformPotentialActions(nextActions, newTrigger, metaData);
                    // We break since the optional actions might change (Some actions aren't allowed after a specific action).
                    yield break;
                }
            }
        }

        private bool IsConditionsMet([NotNull] SwActionData swActionData, string newTrigger)
        {
            return IsMetSearchType(swActionData, newTrigger) && CheckCondition(swActionData.Conditions);
        }

        private static bool IsMetSearchType(SwActionData swActionData, string newTrigger)
        {
            bool isMetSearchType;

            switch (swActionData.SearchType)
            {
                default:
                case ESearchType.OR:
                    isMetSearchType = swActionData.Triggers.Contains(newTrigger);

                    break;
            }

            return isMetSearchType;
        }

        private void OnActionPerformed(SwActionData performedAction)
        {
            if (performedAction == null) return;
            
            CacheActionPerformed(performedAction);
            SwTacUtils.ModifyConditionsOnActionPerformed(performedAction, _settings.isTimeBased);
        }

        private void CacheActionPerformed(SwActionData performedAction)
        {
            CurrentIterationActionsSet.Add(performedAction.ActionType);
            SwInfra.Logger.Log(EWisdomLogType.TAC, $"Current iteration action types performed = ({CurrentIterationActionsSet.SwToString()})");

            CurrentIterationActionsIdsSet.Add(performedAction.Id);
            SwInfra.Logger.Log(EWisdomLogType.TAC, $"Current iteration action ids performed = ({CurrentIterationActionsIdsSet.SwToString()})");
        }
        
        private void SetUiActionTypes()
        {
            _uiActionTypes.Clear();

            foreach (var listener in _listeners)
            {
                if (listener.IsUiActionType)
                {
                    _uiActionTypes.Add(listener.ActionType);
                }   
            }
        }
        
        private SwTacConfig GetMergeConfigs()
        {
            _combinedTriggerData = _configTriggerData;

            if (_partnersActions != null)
            {
                var currentActionsList = _configTriggerData.Actions.ToList();
                currentActionsList.AddRange(_partnersActions);
                _combinedTriggerData.Actions = currentActionsList.ToArray();
            }

            return _combinedTriggerData;
        }
        
        private void AddPartnerAction(SwActionData action)
        {
            _partnersActions ??= new List<SwActionData>();
            _partnersActions.Add(action);
        }

        #endregion
    }
}