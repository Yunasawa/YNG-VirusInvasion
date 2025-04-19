#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SupersonicWisdomSDK
{
    internal static class SwTimerConfigUtils
    {
        #region --- Constants ---

        private const char ENTRY_DELIMITER = ',';
        private const char LEVEL_TO_DURATION_DELIMITER = ':';

        #endregion
        
        
        #region --- Public Methods ---
        
        public static SwTimerConfig ParseTimerConfig(string rawTimerConfig, Func<float, long> normalizationFunc)
        {
            var ret = new SwTimerConfig();

            if (string.IsNullOrEmpty(rawTimerConfig))
            {
                return ret;
            }

            var keyToDuration = new SortedDictionary<long, float>();
            var rangeStrings = rawTimerConfig.Split(ENTRY_DELIMITER);

            if (rangeStrings.Length == 1 && !rangeStrings[0].Contains(LEVEL_TO_DURATION_DELIMITER))
            {
                rangeStrings = new[] { $"0{LEVEL_TO_DURATION_DELIMITER}{rawTimerConfig}" };
            }

            foreach (var rangeString in rangeStrings)
            {
                try
                {
                    var splitted = rangeString.Split(LEVEL_TO_DURATION_DELIMITER);

                    if (splitted.Length == 2)
                    {
                        var rawStart = float.Parse(splitted[0], CultureInfo.InvariantCulture.NumberFormat);
                        var start = normalizationFunc?.Invoke(rawStart) ?? (long)rawStart;
                        var duration = float.Parse(splitted[1], CultureInfo.InvariantCulture.NumberFormat);

                        if (start < 0 || duration < -1)
                        {
                            throw new SwException($"Invalid values for timer: {rangeString}");
                        }

                        if (keyToDuration.ContainsKey(start))
                        {
                            throw new SwException($"Ambiguous key {start} in timer. {rangeString} conflicts with {start}:{keyToDuration[start]}");
                        }

                        keyToDuration[start] = duration;
                    }
                    else
                    {
                        throw new SwException($"Bad range string {rangeString}");
                    }
                }
                catch (Exception e)
                {
                    SwInfra.Logger.LogException(e, EWisdomLogType.Ad, $"An error occurred while parsing timer range {rangeString}.");
                }
            }

            var levelToDurationPairs = keyToDuration.ToArray();
            long nextRangeStart = 0;
            var length = levelToDurationPairs.Length;

            for (var i = length - 1; i >= 0; i--)
            {
                var pair = levelToDurationPairs[i];
                var level = pair.Key;
                var duration = pair.Value;
                
                var isFiniteRange = i < length - 1;
                long? end = null; // value for infinite endLevel

                if (isFiniteRange)
                {
                    end = nextRangeStart - 1;
                }

                var range = new SwRange(level, end);

                if (range.IsValid)
                {
                    ret[range] = duration;
                    nextRangeStart = range.Start;
                }
                else
                {
                    SwInfra.Logger.LogWarning(EWisdomLogType.Ad, $"omitted invalid range {range}");
                }
            }

            return ret;
        }
    }
    
    #endregion
}
#endif