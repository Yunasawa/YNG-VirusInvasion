#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwExternalParamsData
    {
        #region --- Inspector ---

        [SerializeField]
        public bool IsEnabled;

        [SerializeField]
        public List<ESwExternalParamsEvents> events;

        [SerializeField]
        public string key = "";

        [SerializeField]
        public string name = "";

        public SwExternalParamsType Type
        {
            get { return InitialValue._type; }
            set { InitialValue._type = value; }
        }

        [SerializeField]
        public SwParamValue InitialValue;

        public bool IsValid
        {
            get
            {
                return IsEnabled && !key.SwIsNullOrEmpty() && !name.SwIsNullOrEmpty();
            }
        }

        #endregion

        
        #region --- Construction ---

        public SwExternalParamsData(SwExternalParamsType type = SwExternalParamsType.String, List<ESwExternalParamsEvents> events = null)
        {
            this.events = events ?? new List<ESwExternalParamsEvents>();
            InitialValue = new SwParamValue(type);
        }

        #endregion
    }
}
#endif