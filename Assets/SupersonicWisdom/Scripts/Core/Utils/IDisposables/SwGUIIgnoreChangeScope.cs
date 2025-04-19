using System;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwGUIIgnoreChangeScope : IDisposable
    {
        #region --- Members ---

        private readonly bool _isChanged;

        #endregion


        #region --- Construction ---

        public SwGUIIgnoreChangeScope()
        {
            _isChanged = GUI.changed;
        }

        #endregion


        #region --- Public Methods ---

        public void Dispose()
        {
            GUI.changed = _isChanged;
        }

        #endregion
    }
}