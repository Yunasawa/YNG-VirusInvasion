using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwDynamicConfigManager : SwDynamicConfigManagerBase
    {
        #region --- Members ---
        
        private readonly List<ISwDynamicConfigProvider> _configProviders;
        
        #endregion
        
        
        #region --- Construction ---

        public SwDynamicConfigManager(List<ISwDynamicConfigProvider> configProviders)
        {
            _configProviders = configProviders ?? new List<ISwDynamicConfigProvider>();
            LoadConfig();
            RegisterProviderCallbacks();
        }
        
        #endregion
        
        
        #region --- Private Methods ---

        // Loads Initial config from all providers
        private void LoadConfig()
        {
            foreach (var provider in _configProviders)
            {
                if (provider == null) continue;
                
                var config = provider.GetConfig();
                MergeConfig(config, provider.Source, (int)provider.Source);
            }
        }

        // Register callbacks for all providers and provides periodic updates
        private void RegisterProviderCallbacks()
        {
            foreach (var provider in _configProviders)
            {
                if (provider == null) continue;
                
                provider.OnConfigUpdated = updatedConfig => OnProviderConfigUpdated(updatedConfig, provider.Source, (int)provider.Source);
            }
        }
        
        #endregion
    }
}