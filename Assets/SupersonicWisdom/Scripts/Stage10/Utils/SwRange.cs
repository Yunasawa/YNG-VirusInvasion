#if SW_STAGE_STAGE10_OR_ABOVE
using System;

namespace SupersonicWisdomSDK
{
    /// <summary>
    ///     Represents a range of levels/minutes
    /// </summary>
    internal class SwRange : IComparable
    {
        #region --- Properties ---

        public bool IsInfinite
        {
            get { return End == null; }
        }

        public bool IsValid
        {
            get { return Start >= 0 && (IsInfinite || Start <= End); }
        }

        /// <summary>
        ///     Start level/seconds of range (inclusive)
        /// </summary>
        public long Start { get; set; }

        /// <summary>
        ///     End level/seconds of range (inclusive)
        ///     null value represents infinity
        /// </summary>
        public long? End { get; set; }

        #endregion


        #region --- Construction ---

        public SwRange(long start, long? end = null)
        {
            Start = start;
            End = end;
        }

        #endregion


        #region --- Public Methods ---

        public override string ToString ()
        {
            return ToString(false);
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var otherRange = obj as SwRange;

            if (otherRange != null)
            {
                return Start - otherRange.Start > 0 ? 1 : Start - otherRange.Start < 0 ? -1 : 0;
            }

            throw new ArgumentException("Object is not a SwRange");
        }

        public bool IsIn(float level)
        {
            return level >= Start && (IsInfinite || level <= End);
        }

        public string ToString(bool toMinutes)
        {
            var ret = IsValid ? "" : "Invalid Range:";
            ret += $"[{ToStringRangeBoundary(Start, toMinutes)},{ToStringRangeBoundary(End, toMinutes)}]";

            return ret;
        }

        #endregion


        #region --- Private Methods ---

        private static string ToStringRangeBoundary(long? value, bool toMinutes)
        {
            if (value == null)
            {
                return "Infinity";
            }

            if (toMinutes)
            {
                return $"{value / 60f:0.0}";
            }

            return value.ToString();
        }

        #endregion
    }
}
#endif