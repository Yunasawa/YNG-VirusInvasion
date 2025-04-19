#if SW_STAGE_STAGE10_OR_ABOVE

using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwStage10AsyncFlow : SwAsyncFlow, ISwUiToolkitWindowStateListener
    {
        #region --- Construction ---
        
        public SwStage10AsyncFlow(GameObject mono, SwAsyncFlowStep[] steps) : base(mono, steps) { }
        
        #endregion


        #region --- Public Methods ---
        
        public void OnWindowOpened(SwUiToolkitWindow window)
        {
            _timeoutHandler?.PauseTimer();
        }

        public void OnWindowClosed(SwUiToolkitWindow window)
        {
            _timeoutHandler?.ResumeTimer();
        }
        
        #endregion
    }
}

#endif