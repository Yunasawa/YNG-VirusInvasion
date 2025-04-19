#if SW_STAGE_STAGE10_OR_ABOVE

namespace SupersonicWisdomSDK
{
    internal class SwStage10InstallSourceHandlerWindow : SwUiToolkitWindow
    {
        #region --- Properties ---

        internal override bool IsBlockingAds
        {
            get { return true; }
        }

        internal override ESwUiToolkitType Type
        {
            get { return ESwUiToolkitType.InstallSourceBlocker; }
        }

        internal override int Priority
        {
            get { return 80; }
        }

        #endregion


        #region --- Private Methods ---

        protected override void OnDisplay(SwVisualElementPayload[] payload = null)
        {
            SetTitle(payload);
            SetBody(payload);
            SetButton(payload);
        }

        protected override void OnClose()
        {
            SwInfra.Logger.Log(EWisdomLogType.InstallSourceHandler, "Closed");
        }

        #endregion
    }
}
#endif