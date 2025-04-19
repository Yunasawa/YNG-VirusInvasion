#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwStage10ResourcesManager : SwBaseExternalParamsManager, ISwTrackerDataProvider
    {
        #region --- Constants ---

        private const string STORE_KEY = "resources_data";
        internal const string BALANCE_HC = "balanceHc";
        internal const string BALANCE_SOFT_A = "balanceSoftA";
        internal const string BALANCE_SOFT_B = "balanceSoftB";
        internal const string BALANCE_SOFT_C = "balanceSoftC";

        #endregion


        #region --- Properties ---

        protected override List<SwExternalParamsData> Config
        {
            get { return SwResourceManagementUtils.Load()?.All; }
        }

        protected override string StoreKey
        {
            get { return STORE_KEY; }
        }

        protected override EWisdomLogType LogType
        {
            get { return EWisdomLogType.Resource; }
        }

        protected bool IsEnabled
        {
            get
            {
                return 
#if SW_RESOURCE_MANAGEMENT_ENABLED
                    true;
#else
                    false;
#endif
            }
        }
        #endregion
        

        #region --- Public Methods ---

#if SW_RESOURCE_MANAGEMENT_ENABLED
        public void SetData(ESwResourceTypes enumName, object value)
        {
            SetDataByName(enumName.SwToString(), value);
        }
#endif

        public (SwJsonDictionary dataDictionary, IEnumerable<string> keysToEncrypt) ConditionallyAddExtraDataToTrackEvent(SwCoreUserData coreUserData, string eventName = "")
        {
            var extraDataToSendTracker = new Dictionary<string, object>();

            if (IsEnabled)
            {
                extraDataToSendTracker[BALANCE_HC] = KeyToValueDict.SwSafelyGet(ESwWalletParams.sw_balance_hc.SwToString(), null);
                extraDataToSendTracker[BALANCE_SOFT_A] = KeyToValueDict.SwSafelyGet(ESwWalletParams.sw_balance_soft_a.SwToString(), null);
                extraDataToSendTracker[BALANCE_SOFT_B] = KeyToValueDict.SwSafelyGet(ESwWalletParams.sw_balance_soft_b.SwToString(), null);
                extraDataToSendTracker[BALANCE_SOFT_C] = KeyToValueDict.SwSafelyGet(ESwWalletParams.sw_balance_soft_c.SwToString(), null);
                extraDataToSendTracker.RemoveNullValues();
            }
            
            return (new SwJsonDictionary(extraDataToSendTracker), KeysToEncrypt());
            
            IEnumerable<string> KeysToEncrypt()
            {
                yield break;
            }
        }

        #endregion
    }
}
#endif