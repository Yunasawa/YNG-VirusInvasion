using System;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal struct SwEventMetadataDto
    {
        public string bundle;
        public string apiKey;
        public string os;
        public string osVer; // OS version
        public string version;
        public string device;
        public string uuid;
        public string swInstallationId;
        public string appsFlyerId;
        public string abId;
        public string abName;
        public string abVariant;
        public string installDate;
        public string gameId;
        public string sdkVersion;
        public long sdkVersionId;
        public string sdkStage;
        public string feature;
        public long featureVersion;
        public string unityVersion;
        public string attStatus;
        public string noAds;
        public string products;
        public string configStatus;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string deeplink;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string tester;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string installSdkVersion;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long installSdkVersionId;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string installAppVersion;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long installAppVersionId;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string prevOrgId;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string gameName;
        
#if UNITY_IOS
        public string sandbox;
        public string idfv;
#endif
#if UNITY_ANDROID
        public string appSetId;
#endif
    }
}