#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal static class SwExternalParamsUtils
    {
        #region --- Public Methods ---

        public static Dictionary<string, object> InitAndLoadKeyToValueDict(List<SwExternalParamsData> dataList, EWisdomLogType logType)
        {
            var dict = new Dictionary<string, object>();

            if (dataList.SwIsNullOrEmpty())
            {
                return dict;
            }

            foreach (var data in dataList)
            {
                if (data.IsEnabled && !dict.HasConfigKey(data.key))
                {
                    dict.SwAddOrReplace(data.key, data.InitialValue.Value);
                }
            }

            if (!dict.SwIsNullOrEmpty())
            {
                var allEnabledKeysStringify = SwUtils.JsonHandler.SerializeObject(dict);
                SwInfra.Logger.Log(logType, allEnabledKeysStringify);
            }

            return dict;
        }

        #endregion
    }
}
#endif