using System;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwGUIHorizontalScope : IDisposable
    {
        #region --- Members ---

        private readonly int? _extraSpaceUnits;

        #endregion


        #region --- Construction ---

        public SwGUIHorizontalScope(GUIStyle style = null, int? extraSpaceUnits = null)
        {
            _extraSpaceUnits = extraSpaceUnits;

            if (style != null)
            {
                GUILayout.BeginHorizontal(style);
            }
            else
            {
                GUILayout.BeginHorizontal();
            }
        }

        #endregion


        #region --- Public Methods ---

        public void Dispose()
        {
            GUILayout.EndHorizontal();

            if (_extraSpaceUnits != null)
            {
                GUILayout.Space(_extraSpaceUnits.Value);
            }
        }

        #endregion
    }
}