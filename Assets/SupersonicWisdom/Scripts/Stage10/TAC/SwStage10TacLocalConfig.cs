#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    internal class SwStage10TacLocalConfig : SwLocalConfig
    {
        #region --- Constants ---
        
        internal const string TRIGGER_DATA_KEY_ = "swTacActionsData";

        #endregion


        #region --- Members ---

        public static string LocalConfigValue { get; private set; }

        #endregion


        #region --- Properties ---

        public override Dictionary<string, object> LocalConfigValues
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { TRIGGER_DATA_KEY_, LocalConfigValue },
                };
            }
        }

        #endregion


        #region --- Construction ---

        public SwStage10TacLocalConfig()
        {
            LocalConfigValue = GetLocalConfig();
        }

        #endregion


        #region --- Private Methods ---

        protected virtual List<SwActionData> GetActions()
        {
            var actionsList = new List<SwActionData>
            {
                GetAppUpdateAction(),
            };

            return actionsList;
        }

        protected string AddSwPrefix(string key)
        {
            return $"sw_{key}";
        }

        private string GetLocalConfig()
        {
            var localConfig = new SwTacConfig
            {
                MaximumUiActions = 2,
                Actions = GetActions().ToArray(),
            };
            
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };

            return SwUtils.JsonHandler.SerializeObject(localConfig, settings);
        }

        private SwActionData GetAppUpdateAction()
        {
            return new SwActionData
            {
                Id = "Show-app-update",
                Priority = 20000,
                ActionType = ESwActionType.ShowAppUpdate,
                SearchType = ESearchType.OR,
                Triggers = new[] { AddSwPrefix(SwBeforeReadyTriggerStep.BEFORE_READY) },
                Conditions = new SwCoreTacConditions
                {
                    ConfigStatus = new[]
                    {
                        EConfigStatus.Remote,
                    },
                },
            };
        }

        #endregion
    }
}
#endif