#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwParamValue
    {
        #region --- Inspector ---

        [SerializeField]
        public SwExternalParamsType _type;

        [SerializeField]
        private bool _bool;

        [SerializeField]
        private float _float;

        [SerializeField]
        private int _int;

        [SerializeField]
        private string _string;

        #endregion


        #region --- Properties ---

        public object Value
        {
            get
            {
                return _type switch
                {
                    SwExternalParamsType.String => _string,
                    SwExternalParamsType.Bool => _bool,
                    SwExternalParamsType.Int => _int,
                    SwExternalParamsType.Float => _float,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            set
            {
                switch (_type)
                {
                    case SwExternalParamsType.String:
                        value ??= "";

                        _string = (string)value;

                        break;
                    case SwExternalParamsType.Bool:
                        value ??= false;

                        _bool = (bool)value;

                        break;
                    case SwExternalParamsType.Int:
                        value ??= 0;

                        _int = (int)value;

                        break;
                    case SwExternalParamsType.Float:
                        value ??= 0f;

                        _float = (float)value;

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        #endregion


        #region --- Construction ---

        public SwParamValue(SwExternalParamsType type)
        {
            _type = type;
        }

        #endregion
    }
}
#endif