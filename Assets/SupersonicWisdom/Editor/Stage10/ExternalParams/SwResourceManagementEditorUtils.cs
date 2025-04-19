#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwResourceManagementEditorUtils
    {
        #region --- Constants ---
        
        private const string RESOURCE_API_ENUM_NAME = "ESwResourceTypes";
        private const int RESOURCE_API_STAGE = 10;
        private const string RESOURCE_MANAGEMENT_ASSET_PATH = SwResourceManagementUtils.ASSET_PATH;

        #endregion


        #region --- Private Methods ---

        internal static GUIStyle GetRegularBoxStyle(Color backgroundColor)
        {
            return new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = 14,
                normal =
                {
                    textColor = SwColors.WHITE,
                    background = SwUiUtils.MakeTex(2, 2, backgroundColor),
                },
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(SwBaseExternalParamsViewController.EXTRA_SPACE_UNITS, SwBaseExternalParamsViewController.EXTRA_SPACE_UNITS, SwBaseExternalParamsViewController.EXTRA_SPACE_UNITS, SwBaseExternalParamsViewController.EXTRA_SPACE_UNITS),
                margin = new RectOffset(SwBaseExternalParamsViewController.EXTRA_SPACE_UNITS, SwBaseExternalParamsViewController.EXTRA_SPACE_UNITS, SwBaseExternalParamsViewController.EXTRA_SPACE_UNITS, SwBaseExternalParamsViewController.EXTRA_SPACE_UNITS),
                border = new RectOffset(4, 4, 4, 4),
            };
        }

        internal static SwResourcesManagementExternalParamsData CreateOrLoad()
        {
            var (asset, isNew) = 
                SwFileUtils.CreateScriptableObjectInstance<SwResourcesManagementExternalParamsData>(RESOURCE_MANAGEMENT_ASSET_PATH);
            
            asset.Wallet = InitWallet(asset.Wallet);

            return asset;
        }

        private static SwExternalParamsType GetWalletParamType(ESwWalletParams param)
        {
            switch (param)
            {
                case ESwWalletParams.sw_balance_hc:
                case ESwWalletParams.sw_balance_soft_a:
                case ESwWalletParams.sw_balance_soft_b:
                case ESwWalletParams.sw_balance_soft_c:
                    return SwExternalParamsType.Int;
                case ESwWalletParams.none:
                    throw new ArgumentOutOfRangeException(nameof(param), param, $"None has not type");
                default:
                    throw new ArgumentOutOfRangeException(nameof(param), param, $"Please add type for wallet param - {param}");
            }
        }

        internal static SwExternalParamsData GetParamDataObjectByType(SwExternalParamsType type)
        {
            switch (type)
            {
                case SwExternalParamsType.String:
                    return new SwExternalParamsData();
                case SwExternalParamsType.Bool:
                    return new SwExternalParamsData(SwExternalParamsType.Bool);
                case SwExternalParamsType.Int:
                    return new SwExternalParamsData(SwExternalParamsType.Int);
                case SwExternalParamsType.Float:
                    return new SwExternalParamsData(SwExternalParamsType.Float);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), $"Please add default data object for type -{type}");
            }
        }

        private static List<SwExternalParamsData> InitWallet(List<SwExternalParamsData> externalParamsData)
        {
            externalParamsData ??= new List<SwExternalParamsData>();
            
            foreach (var walletParam in Enum.GetValues(typeof(ESwWalletParams)))
            {
                var walletParamEnum = (ESwWalletParams)walletParam;
                
                // This line handled sdk upgrade when new predefined resources params was added
                var isParamInitialized = externalParamsData.Exists((param) => param.key == walletParamEnum.SwToString());
                
                if (walletParamEnum == ESwWalletParams.none || isParamInitialized)
                {
                    continue;
                }

                var obj = GetParamDataObjectByType(GetWalletParamType(walletParamEnum));
                obj.key = walletParamEnum.SwToString();
                externalParamsData.Add(obj);
            }

            return externalParamsData;
        }


        public static void TryGenerateEnum()
        {
            var configuredParams = SwResourceManagementUtils.Load()?.All?.Where(param => param.IsEnabled).Select(param => param.name).ToArray();

            if (configuredParams.SwIsNullOrEmpty())
            {
                SwFileUtils.GenerateEnum(RESOURCE_API_STAGE, RESOURCE_API_ENUM_NAME, configuredParams);
                return;
            }

            var currentEnum = SwEnumUtils.GetEnumValues<ESwResourceTypes>().ToArray();
            var needsRegeneration = configuredParams.Any(configuredParam => !currentEnum.Contains(configuredParam));

            if (needsRegeneration)
            {
                SwFileUtils.GenerateEnum(RESOURCE_API_STAGE, RESOURCE_API_ENUM_NAME, configuredParams);
            }
        }

        #endregion
    }
}
#endif