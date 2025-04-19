#if SW_STAGE_STAGE10_OR_ABOVE
namespace SupersonicWisdomSDK
{
    internal class SwStage10WisdomRemoteConfigHandler : SwWisdomRemoteConfigHandler
    {
        #region --- Fields ---
        
        private readonly SwDeepLinkHandler _deepLinkHandler;
        
        #endregion
        
        
        #region --- Constructor ---
        
        public SwStage10WisdomRemoteConfigHandler(ISwSettings settings, SwCoreUserData coreUserData, SwCoreTracker tracker, SwCoreNativeAdapter nativeAdapter, SwDeepLinkHandler deepLinkHandler) : base(settings, coreUserData, tracker, nativeAdapter)
        {
            _deepLinkHandler = deepLinkHandler;
        }
        
        #endregion
        
        
        #region --- Private Methods ---
        
        protected override SwRemoteConfigRequestPayload CreatePayload()
        {
            var payload = base.CreatePayload();
            
            payload.abTestVariant = TryDeeplinkForAbTestVariant();
            
            return payload;
        }

        private string TryDeeplinkForAbTestVariant()
        {
            if (_deepLinkHandler?.DeepLinkParams == null) return null;
            
            var containAbVariant = _deepLinkHandler.DeepLinkParams.TryGetValue(SwStage10DeepLinkConstants.AB_TEST_VARIANT, out var result);
            var variant = containAbVariant ? result : null;
            
            return variant;
        }
        
        #endregion
    }
}
#endif