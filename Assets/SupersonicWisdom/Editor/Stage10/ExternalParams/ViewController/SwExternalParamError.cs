#if SW_STAGE_STAGE10_OR_ABOVE
namespace SupersonicWisdomSDK.Editor
{
    internal class SwParamError
    {
        #region --- Properties ---

        public ESwExternalParamsParamError Error { get; }
        public SwBaseExternalParams Key { get; }

        #endregion


        #region --- Construction ---

        public SwParamError(SwBaseExternalParams key, ESwExternalParamsParamError error)
        {
            Key = key;
            Error = error;
        }

        #endregion
    }
}
#endif