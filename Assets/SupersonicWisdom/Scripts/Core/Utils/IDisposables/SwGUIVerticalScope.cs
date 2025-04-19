using System;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwGUIVerticalScope : IDisposable
    {
        #region --- Members ---

        private readonly int? _extraSpaceUnits;

        #endregion


        #region --- Construction ---

        public SwGUIVerticalScope(GUIStyle style = null, int? extraSpaceUnits = null)
        {
            _extraSpaceUnits = extraSpaceUnits;

            if (style != null)
            {
                GUILayout.BeginVertical(style);
            }
            else
            {
                GUILayout.BeginVertical();
            }
        }

        #endregion


        #region --- Public Methods ---

        public void Dispose()
        {
            GUILayout.EndVertical();

            if (_extraSpaceUnits != null)
            {
                GUILayout.Space(_extraSpaceUnits.Value);
            }
        }

        #endregion
    }
}