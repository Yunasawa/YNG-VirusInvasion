#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;
using System.Linq;

namespace SupersonicWisdomSDK.Editor
{
    [Serializable]
    internal class SwWalletExternalParamsViewController : SwPredefinedExternalParamsViewController
    {
        #region --- Constants ---

        private const string RESOURCES_LABEL_TEXT = "Resources";

        #endregion


        #region --- Members ---

        private SwExternalParamsPredefinedSection _view;

        #endregion


        #region --- Properties ---

        public virtual string Name
        {
            get { return RESOURCES_LABEL_TEXT; }
        }

        #endregion


        #region --- Construction ---

        public SwWalletExternalParamsViewController(List<SwExternalParamsData> rawConfig)
        {
            var options = Enum.GetNames(typeof(ESwWalletParams)).ToList();
            
            // Removing index 0 since its 'none' - the lowest enum in ESwWalletParams
            options.RemoveAt(0);

            foreach (var config in rawConfig)
            {
                var newParam = new SwPredefinedExternalParams(config, options.ToArray());
                newParam.IsEnabledChangedEvent += (type, isEnabled) => { _isEnabledDict.SwAddOrReplace(type, isEnabled); };
                Params.Add(newParam);
                _isEnabledDict.SwAddOrReplace(config.key, config.IsEnabled);
            }

            _view = new SwExternalParamsPredefinedSection(Params, _regularBoxStyle, _errorBoxStyle, EXTRA_SPACE_UNITS, IsAllowed);
        }

        #endregion


        #region --- Public Methods ---

        public bool IsAllowed(string paramType)
        {
            switch (paramType)
            {
                case nameof(ESwWalletParams.none):
                    return false;
                case nameof(ESwWalletParams.sw_balance_hc):
                    return true;
                case nameof(ESwWalletParams.sw_balance_soft_a):
                    return IsEnabled(nameof(ESwWalletParams.sw_balance_hc));
                case nameof(ESwWalletParams.sw_balance_soft_b):
                    return IsEnabled(nameof(ESwWalletParams.sw_balance_hc))
                           && IsEnabled(nameof(ESwWalletParams.sw_balance_soft_a));
                case nameof(ESwWalletParams.sw_balance_soft_c):
                    return IsEnabled(nameof(ESwWalletParams.sw_balance_hc))
                           && IsEnabled(nameof(ESwWalletParams.sw_balance_soft_a))
                           && IsEnabled(nameof(ESwWalletParams.sw_balance_soft_b));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion


        #region --- Private Methods ---

        protected override void Draw(List<SwParamError> errors)
        {
            _view.Draw(errors);
        }

        #endregion
    }
}
#endif