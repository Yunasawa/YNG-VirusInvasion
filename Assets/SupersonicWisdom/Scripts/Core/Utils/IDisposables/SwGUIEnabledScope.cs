using System;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwGUIEnabledScope : IDisposable
    {
        #region --- Members ---

        private readonly bool _previousState;

        #endregion


        #region --- Construction ---

        public SwGUIEnabledScope(bool newState)
        {
            _previousState = GUI.enabled;
            GUI.enabled = newState;
        }

        #endregion


        #region --- Public Methods ---

        public void Dispose()
        {
            GUI.enabled = _previousState;
        }

        #endregion
    }
}