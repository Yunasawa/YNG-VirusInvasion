using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwAbConfig
    {
        #region --- Inner classes ---

        internal static class Constants
        {
            #region --- Constants ---

            public const string RELEASED = "Released";
            
            /// The default is eternal
            public const int DEFAULT_DURATION_DAYS = int.MaxValue;

            internal const string USER_WISDOM_AB_STATUS = "userWisdomAbStatus";
            internal const string USER_WISDOM_AB_INACTIVE = "0";
            internal const string USER_WISDOM_AB_ACTIVE = "1";
            
            #endregion
        }

        internal enum AbStatus
        {
            Inactive, //The order matter
            Active,
            None,
        }
        
        #endregion
        

        #region --- Members ---

        [JsonProperty("group")]
        public string group;
        [JsonProperty("id")]
        public string id;
        [JsonProperty("key")]
        public string key;
        [JsonProperty("value")]
        public string value;
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)] 
        public string status;

        /// Date format: yyyy-MM-dd
        [JsonProperty("expirationDate")]
        public string expirationDate;

        [JsonProperty("durationDays", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(Constants.DEFAULT_DURATION_DAYS)]
        public int durationDays;

        [JsonIgnore]
        [NonSerialized]
        private int? _passedDays;

        [JsonIgnore] 
        [NonSerialized] 
        private string _userAbStatus;

        public int? RemainingDays { get; private set; }

        [JsonIgnore]
        private int PassedDays
        {
            get
            {
                _passedDays ??= CalculatePassedDays();

                return _passedDays.Value;
            }
        }

        private int CalculatePassedDays()
        {
            var today = SwUtils.DateAndTime.TryParseDate(TodayString, SwConstants.SORTABLE_DATE_STRING_FORMAT);

            if (!today.HasValue) return -1;

            var startDayString = SwInfra.KeyValueStore.GetString(StartDateStorageKey(id), null);
            var startDay = SwUtils.DateAndTime.TryParseDate(startDayString, SwConstants.SORTABLE_DATE_STRING_FORMAT);
            var passedDays = -1;

            if (startDay.HasValue)
            {
                passedDays = today.Value.Subtract(startDay.Value).Days + 1;
                SwInfra.Logger.Log(EWisdomLogType.Config, $"passed days of A/B {id}: {passedDays}");
            }

            return passedDays;
        }

        #endregion


        #region --- Properties ---

        [JsonIgnore]
        private readonly Dictionary<string, int?> _cachedCalculation = new Dictionary<string, int?>();

        [JsonIgnore]
        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(id); }
        }

        [JsonIgnore]
        public bool IsEternal
        {
            get { return RemainingDays == null; }
        }

        [JsonIgnore]
        public bool? IsInactive
        {
            get
            {
                switch (UserAbStatus)
                {
                    case AbStatus.Active: 
                        return false;
                    case AbStatus.Inactive:
                        return true;
                    case AbStatus.None:
                    default:
                        return null;
                };
            }
        }
        
        public bool IsActive
        {
            get
            {
                return UserAbStatus == AbStatus.Active;
            }
        }

        [JsonIgnore]
        private AbStatus UserAbStatus
        {
            get
            {
                _userAbStatus ??= SwInfra.KeyValueStore.GetString(Constants.USER_WISDOM_AB_STATUS, null);

                return _userAbStatus switch
                {
                    Constants.USER_WISDOM_AB_INACTIVE => AbStatus.Inactive,
                    Constants.USER_WISDOM_AB_ACTIVE => AbStatus.Active,
                    _ => AbStatus.None,
                };
            }

            set
            {
                _userAbStatus = ((int)value).SwToString();
                SwInfra.KeyValueStore.SetString(Constants.USER_WISDOM_AB_STATUS, _userAbStatus).Save();
            }
        }

        #endregion


        #region --- Public Methods ---

        public override string ToString()
        {
            return SwUtils.JsonHandler.SerializeObject(this);
        }

        public void UpdateVariantStatus()
        {
            UpdateRemainingDays();
            
            if (IsInactive == true)
            {
                InvalidateValues();
                
                SwInfra.Logger.Log(EWisdomLogType.Config, "Variant is inactive");
                
                return;
            }
            
            if (!group.SwIsNullOrEmpty() && IsInactive == null)
            {
                UserAbStatus = AbStatus.Active;
            }
            else if (status is Constants.RELEASED || RemainingDays <= 0)
            {
                InvalidateValues();
                
                UserAbStatus = AbStatus.Inactive;
                
                SwInfra.Logger.Log(EWisdomLogType.Config, "Variant Inactive");
            }
        }
        
        public string GetTrackEventAbStatus()
        {
            return IsInactive == true ? Constants.USER_WISDOM_AB_INACTIVE : Constants.USER_WISDOM_AB_ACTIVE;
        }

        #endregion


        #region --- Private Methods ---
        
        private void InvalidateValues()
        {
            value = group = key = id = string.Empty;
        }
        
        private int? UpdateRemainingDays()
        {
            SwInfra.Logger.Log(EWisdomLogType.Config, $"ab: {this}");
            RemainingDays = CalculateRemainingDays();
            SwInfra.Logger.Log(EWisdomLogType.Config, $"remainingDays: {RemainingDays?.ToString() ?? "eternal"}");
            SwInfra.Logger.Log(EWisdomLogType.Config, $"ab key = {key} | ab value = {value} | ab status = {status}");

            return RemainingDays;
        }
        
        private static string StartDateStorageKey(string id)
        {
            return "StartDateForAb_" + id;
        }

        private void TryCacheStartDate()
        {
            if (!IsValid) return;

            var startDay = SwInfra.KeyValueStore.GetString(StartDateStorageKey(id), null);

            if (!string.IsNullOrEmpty(startDay)) return;

            // Today's start date only, at midnight.
            startDay = TodayString;
            SwInfra.KeyValueStore.SetString(StartDateStorageKey(id), startDay).Save();
        }

        private static DateTime TodayAtMidnight
        {
            get
            {
                return DateTime.ParseExact(TodayString, SwConstants.SORTABLE_DATE_STRING_FORMAT,
                    CultureInfo.InvariantCulture);
            }
        }

        private static string TodayString
        {
            get { return DateTime.Now.SwToString(SwConstants.SORTABLE_DATE_STRING_FORMAT); }
        }

        private int? CalculateRemainingDays()
        {
            var todayKey = TodayString;
            var alreadyCalculated = _cachedCalculation.ContainsKey(todayKey);

            if (alreadyCalculated) return _cachedCalculation.SwSafelyGet(todayKey, null);

            int? remainingDays = null;

            if (IsValid)
            {
                TryCacheStartDate();
                
                if (durationDays >= 0 && durationDays != Constants.DEFAULT_DURATION_DAYS)
                {
                    remainingDays = durationDays - PassedDays;
                }

                var terminationDate = SwUtils.DateAndTime.TryParseDate(expirationDate, SwConstants.SORTABLE_DATE_STRING_FORMAT);

                if (terminationDate != null)
                {
                    var diffDays = terminationDate.Value.Subtract(TodayAtMidnight).Days;

                    if ((remainingDays ?? Constants.DEFAULT_DURATION_DAYS) > diffDays)
                    {
                        remainingDays = diffDays;
                    }
                }
            }

            _cachedCalculation.Add(todayKey, remainingDays);

            return remainingDays;
        }

        #endregion


        #region --- Event Methods ---

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            UpdateVariantStatus();
        }

        #endregion
    }
}