#if UNITY_ANDROID
using System;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    public class SwAndroidSslApply : IPostGenerateGradleAndroidProject
    {
        #region --- Constants ---
        
        private const int CALLBACK_ORDER = int.MaxValue;
        private const string NETWORK_SECURITY_CONFIG_VALUE_DEBUG = "@xml/sw_network_security_config";
        private const string NETWORK_SECURITY_CONFIG_VALUE_RELEASE = "@xml/sw_network_security_config_release";
        private const string NETWORK_SECURITY_CONFIG_TAG = "networkSecurityConfig";
        
        #endregion
        
        
        #region --- Properties ---
        
        public int callbackOrder { get; } = CALLBACK_ORDER;
        
        #endregion
        
        
        #region --- Members ---
        
        private static readonly XName NetworkSecurityConfigAttrName = SwAndroidManifestUtils.AndroidNamespace + NETWORK_SECURITY_CONFIG_TAG;
        private static readonly XName DebuggableAttrName = SwAndroidManifestUtils.AndroidNamespace + SwAndroidManifestUtils.DEBUGGABLE_ATTR;

        #endregion
        
        
        #region --- Public Methods ---

        public static bool ApplyNetworkDebug()
        {
            var manifest = SwAndroidManifestUtils.LoadManifest();
            var elemManifest = manifest.Element(SwAndroidManifestUtils.MANIFEST_ELEMENT);
    
            if (elemManifest == null)
            {
                Debug.LogWarning($"Can't load Android manifest file");
                return false;
            }
    
            var elemApplication = elemManifest.Element(SwAndroidManifestUtils.APPLICATION_ELEMENT);
            var releaseBuild = EditorUserBuildSettings.buildAppBundle;
            var buildSettingsConfiguration =  releaseBuild ? NETWORK_SECURITY_CONFIG_VALUE_RELEASE : NETWORK_SECURITY_CONFIG_VALUE_DEBUG;
            var currentNetworkSecurityConfig = elemApplication?.Attribute(NetworkSecurityConfigAttrName)?.Value;

            if (!currentNetworkSecurityConfig.SwIsNullOrEmpty() && (currentNetworkSecurityConfig != NETWORK_SECURITY_CONFIG_VALUE_DEBUG && currentNetworkSecurityConfig != NETWORK_SECURITY_CONFIG_VALUE_RELEASE))
            {
                Debug.LogWarning($"The network_security_config attribute is set to a custom value: '{currentNetworkSecurityConfig}'. We are going to replace it with the default value, if you think this is an issue please contact support.");
            }
    
            elemApplication?.SetAttributeValue(NetworkSecurityConfigAttrName, buildSettingsConfiguration);
            
            var currentDebuggableAttr = elemApplication?.Attribute(DebuggableAttrName);

            if (!releaseBuild)
            {
                if (currentDebuggableAttr == null || !string.Equals(currentDebuggableAttr.Value, true.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    elemApplication?.SetAttributeValue(DebuggableAttrName, true.ToString().ToLower());
                }
            }
            else
            {
                if (currentDebuggableAttr != null && string.Equals(currentDebuggableAttr.Value, true.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    elemApplication?.SetAttributeValue(DebuggableAttrName, false.ToString().ToLower());
                }
            }

            elemManifest.Save(SwAndroidManifestUtils.GetAndroidManifestPath());
            
            return elemApplication?.Attribute(NetworkSecurityConfigAttrName)?.Value == buildSettingsConfiguration;
        }
        
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            ApplyNetworkDebug();
        }
        
        #endregion
    }
}
#endif
