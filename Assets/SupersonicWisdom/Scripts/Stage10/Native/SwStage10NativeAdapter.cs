#if SW_STAGE_STAGE10_OR_ABOVE

namespace SupersonicWisdomSDK
{
    internal class SwStage10NativeAdapter : SwCoreNativeAdapter, ISwStage10ConfigListener
    {
        #region --- Members ---

        protected SwStage10TestUserState _testUserState;

        #endregion


        #region --- Construction ---

        public SwStage10NativeAdapter(ISwNativeApi wisdomNativeApi, ISwSettings settings, SwCoreUserData coreUserData, SwStage10TestUserState testUserState, SwNativeAdditionalDataProvider nativeAdditionalDataProvider) : base(wisdomNativeApi, settings, coreUserData, nativeAdditionalDataProvider)
        {
            _testUserState = testUserState;
        }

        #endregion


        #region --- Public Methods ---

        public void OnConfigResolved(ISwStage10InternalConfig internalConfig, ISwConfigManagerState state, ISwConfigAccessor swConfigAccessor, ESwConfigSource source)
        {
            // Must be before base class
            var eventsConfiguration = internalConfig.Events;
            StoreNativeConfig(eventsConfiguration);
            UpdateConfig();
            
            base.OnConfigResolved(internalConfig, state, swConfigAccessor, source);
        }

        #endregion


        #region --- Private Methods ---

        protected override SwEventMetadataDto GetEventMetadata()
        {
            var eventMetadata = base.GetEventMetadata();
            var userData = (SwStage10UserData)CoreUserData;
            
            eventMetadata.appsFlyerId = userData.AppsFlyerId;
            eventMetadata.tester = _testUserState.IsTester ? "1" : "0";
            eventMetadata.deeplink = _testUserState.IsActivatedByDeeplink ? "1" : "0";

            return eventMetadata;
        }

        #endregion
    }
}
#endif