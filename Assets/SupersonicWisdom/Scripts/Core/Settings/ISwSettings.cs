using System.Collections.Generic;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    internal interface ISwSettings : ISwDynamicConfigProvider
    {
        #region --- Public Methods ---

        string GetAppKey ();

        string GetGameId ();

        string GetGameName();
        
        void Init();

        bool IsDebugEnabled ();

        bool IsPrivacyPolicyEnabled ();

        void OverwritePartially(IReadOnlyDictionary<string, object> dict, [NotNull] ISwKeyValueStore keyValueStore);
        
        Dictionary<string, object> GetValues();
        
        #endregion
    }
}