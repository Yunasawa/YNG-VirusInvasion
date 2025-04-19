#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;
using System.Linq;

namespace SupersonicWisdomSDK
{
    /// <summary>
    ///     Maps range of levels/minutes to duration
    /// </summary>
    internal class SwTimerConfig : SortedDictionary<SwRange, float>
    {
        #region --- Properties ---

        public bool IsEmpty
        {
            get { return !this.Any(); }
        }

        #endregion


        #region --- Public Methods ---

        public override string ToString ()
        {
            return ToString(false);
        }

        /// <summary>
        ///     Returns the corresponding duration for level/seconds
        ///     if level does not match any of the ranges it returns -1 (disabled)
        /// </summary>
        /// <param name="level">The level/seconds to check</param>
        /// <returns>duration</returns>
        public float ResolveDuration(float level)
        {
            foreach (var pair in this)
            {
                var range = pair.Key;

                if (range.IsIn(level))
                {
                    return pair.Value;
                }
            }

            return -1;
        }

        public string ToString(bool toMinutes)
        {
            if (IsEmpty) return "Empty";
            var parts = this.ToArray().Select(pair => $"{GetRange(pair).ToString(toMinutes)}:{GetDuration(pair)}").ToArray();

            return string.Join(",", parts);
        }

        #endregion


        #region --- Private Methods ---

        private static float GetDuration(KeyValuePair<SwRange, float> pair)
        {
            return pair.Value;
        }

        private static SwRange GetRange(KeyValuePair<SwRange, float> pair)
        {
            return pair.Key;
        }

        #endregion
    }
}
#endif