#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwResourcesManagementExternalParamsData : ScriptableObject
    {
        #region --- Inspector ---

        [SerializeField] protected List<SwExternalParamsData> _wallet;

        #endregion


        #region --- Properties ---

        public List<SwExternalParamsData> All
        {
            get
            {
                var all = new List<SwExternalParamsData>();

                if (Wallet != null)
                {
                    all.AddRange(Wallet);
                }

                return all.Where(data => data.IsValid).ToList();
            }
        }

        public List<SwExternalParamsData> Wallet
        {
            get { return _wallet; }
            set { _wallet = value; }
        }

        #endregion
    }
}
#endif