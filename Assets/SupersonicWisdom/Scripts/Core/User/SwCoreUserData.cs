using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwCoreUserData : ISwUserData, ISwTrackerDataProvider, ISwDeepLinkListener
    {
        #region --- Constants ---

        private const string USER_STATE_STORAGE_KEY = "SupersonicWisdomUserState";
        private const string MAIN_LEVEL_WISDOM_ANALYTICS_KEY = "mainLevel";
        private const string TOTAL_LTV_GAME_SESSION_KEY = "totalLtvGameSession";
        private const string TOTAL_LTV_GAME_FAILURE_KEY = "totalLtvGameFailure";
        private const string IS_TIME_BASED_ANALYTICS_KEY = "isTimeBasedGame";
        #endregion


        #region --- Events ---

        internal delegate void OnUserStateChange(SwUserState newState, SwUserStateChangeReason reason);

        internal event OnUserStateChange OnUserStateChangeEvent;

        #endregion


        #region --- Members ---

        public readonly string BundleIdentifier;

        public readonly string Language;
        public readonly string Platform;
        public string Country;

        protected readonly SwSettings Settings;
        private readonly ISwAdvertisingIdsGetter _idsGetter;
        private long _installDateInSeconds;
        private string _customUuid;

        private string _installDateTimeString;

        private SwUserState _userState;
        private bool _completeInitOnce;

        private SwUserActiveDay _activeDay;
        private string _installSdkVersion;
        private string _installAppVersion;
        private string _organizationAdvertisingId;
        private string _previousOrganizationAdvertisingId;

        #endregion


        #region --- Properties ---

        public Tuple<EConfigListenerType, EConfigListenerType> ListenerType
        {
            get { return new Tuple<EConfigListenerType, EConfigListenerType>(EConfigListenerType.FinishWaitingForRemote, EConfigListenerType.GameStarted); }
        }

        /// <summary>
        ///     Indicates if this is a new running app session (not the user's session that affected by foreground / background
        ///     changes).
        /// </summary>
        public bool IsNew { private set; get; }
        
        public DateTime InstallDateTime { private set; get; }
        
        public long InstallDateInSeconds
        {
            get { return _installDateInSeconds; }
            private set
            {
                _installDateInSeconds = value;
                SwInfra.KeyValueStore.SetString(SwStoreKeys.InstallTime, _installDateInSeconds.ToString());
                SwInfra.KeyValueStore.Save();
            }
        }

        public string OrganizationAdvertisingId
        {
            get
            {
                if (_organizationAdvertisingId.SwIsNullOrEmpty())
                {
                    _organizationAdvertisingId = SwInfra.KeyValueStore.GetString(SwStoreKeys.CurrentOrganizationAdvertisingId);
                }
                
                return _organizationAdvertisingId;
            }
            private set
            {
                _organizationAdvertisingId = value;
                SwInfra.KeyValueStore.SetString(SwStoreKeys.CurrentOrganizationAdvertisingId, _organizationAdvertisingId);
                SwInfra.KeyValueStore.Save();
            }
        }
        
        public string PreviousOrganizationAdvertisingId
        {
            get
            {
                if (_previousOrganizationAdvertisingId.SwIsNullOrEmpty())
                {
                    _previousOrganizationAdvertisingId = SwInfra.KeyValueStore.GetString(SwStoreKeys.PreviousOrganizationAdvertisingId);
                }
                
                return _previousOrganizationAdvertisingId;
            }
            private set
            {
                _previousOrganizationAdvertisingId = value;
                SwInfra.KeyValueStore.SetString(SwStoreKeys.PreviousOrganizationAdvertisingId, _previousOrganizationAdvertisingId);
                SwInfra.KeyValueStore.Save();
            }
        }

        public string Uuid { get; private set; }

        public string CustomUuid
        {
            get { return _customUuid; }
            private set
            {
                _customUuid = value;
                SwInfra.KeyValueStore.SetString(SwStoreKeys.CustomUuid, _customUuid);
                SwInfra.KeyValueStore.Save();
            }
        }

        public string InstallSdkVersion
        {
            get { return _installSdkVersion; }
            private set
            {
                _installSdkVersion = value;
                SwInfra.KeyValueStore.SetString(SwStoreKeys.InstallSdkVersion, _installSdkVersion);
                SwInfra.KeyValueStore.Save();
            }
        }
        
        public long InstallSdkVersionId { get; private set; }
        
        public string InstallDate
        {
            get
            {
                return InstallDateTime.SwToString(SwConstants.INSTALL_DATE_FORMAT);
            }
        }
        
        public int ActiveDay
        {
            get
            {
                return _activeDay?.ActiveDay ?? 0;
            }
        }

        public string InstallDateTimeString
        {
            get { return _installDateTimeString; }
            private set
            {
                _installDateTimeString = value;
                SwInfra.KeyValueStore.SetString(SwStoreKeys.InstallDate, _installDateTimeString);
                SwInfra.KeyValueStore.Save();
            }
        }
        
        public string InstallAppVersion
        {
            get { return _installAppVersion; }
            private set
            {
                _installAppVersion = value;
                SwInfra.KeyValueStore.SetString(SwStoreKeys.InstallAppVersion, _installAppVersion);
                SwInfra.KeyValueStore.Save();
            }
        }
        
        public long InstallAppVersionId { get; private set; }
        
        private SwUserState UserState
        {
            get
            {
                _userState ??= new SwUserState();
                return _userState;
            }
        }
        
        #endregion


        #region --- Construction ---

        public SwCoreUserData(SwSettings settings, ISwAdvertisingIdsGetter idsGetter, SwUserActiveDay activeDay)
        {
            Settings = settings;
            _idsGetter = idsGetter;
            _activeDay = activeDay;

            Language = SwUtils.LangAndCountry.GetSystemLanguageIso6391();
            Country = SwUtils.LangAndCountry.GetCountry();
            BundleIdentifier = Application.identifier;
            Platform = Application.platform.ToString();
        }

        #endregion


        #region --- Public Methods ---

        public virtual void Load(ISwInitParams initParams)
        {
            LoadInstallDate();
            LoadActiveDays();
            LoadCustomUuid(); 
            LoadInstallSdkVersion();
            LoadInstallAppVersion();
            LoadAdvertisingIds();
            LoadUserState();
            StartActiveDayProcess();
        }
        
        public long GetSecondsFromInstall()
        {
            if (InstallDateInSeconds == 0)
            {
                return -1;
            }

            return SwUtils.DateAndTime.GetTotalSeconds(DateTime.UtcNow) - InstallDateInSeconds;
        }
        
        internal int LoadUserBucket(string feature, int bucketCount)
        {
            var userBucket = SwInfra.KeyValueStore.GetInt(SwStoreKeys.UserBucket.Format(feature), -1);
            
            if (userBucket == -1)
            {
                userBucket = SwUtils.Crypto.GetBucketGroup(feature, CustomUuid, bucketCount);
                SwInfra.KeyValueStore.SetInt(SwStoreKeys.UserBucket.Format(feature), userBucket);
                SwInfra.KeyValueStore.Save();
            }

            return userBucket;
        }

        public void LoadAdvertisingIds()
        {
            LoadUuid();
            HandleOrganizationAdvertisingId();
        }

        public void LoadUuid()
        {
            Uuid = _idsGetter.GetAdvertisingId();
            SwInfra.Logger.Log(EWisdomLogType.User, $"Got advertising ID ('{Uuid}')");
        }

        private void HandleOrganizationAdvertisingId()
        {
            var nativeOrganizationAdvertisingId = _idsGetter.GetOrganizationAdvertisingId();
            
            SwInfra.Logger.Log(EWisdomLogType.User, $"Got organization advertising ID ('{nativeOrganizationAdvertisingId}')");

            if (OrganizationAdvertisingId.SwIsNullOrEmpty())
            {
                OrganizationAdvertisingId = nativeOrganizationAdvertisingId;
            }
            else if (OrganizationAdvertisingId != nativeOrganizationAdvertisingId)
            {
                PreviousOrganizationAdvertisingId = OrganizationAdvertisingId;
                OrganizationAdvertisingId = nativeOrganizationAdvertisingId;
                
                SwInfra.Logger.Log(EWisdomLogType.User, $"Previous organization advertising ID changed. old: ('{PreviousOrganizationAdvertisingId}') new: ('{nativeOrganizationAdvertisingId}')");
            }
        }

        public bool ModifyUserStateSync(Action<SwUserState> modifier)
        {
            return ModifyUserStateSync(s =>
            {
                modifier.Invoke(s);

                return true;
            });
        }

        public bool ModifyUserStateSync(Func<SwUserState, bool> modifier)
        {
            var copyOfUserState = UserState.Copy();
            var didChange = modifier.Invoke(copyOfUserState);

            if (didChange)
            {
                _userState = copyOfUserState;
                PersistUserState();
            }

            return didChange;
        }

        public bool UpdateAge(SwUserState userState)
        {
            var didChange = false;
            var currentAge = CalculateCurrentAgeDays();

            if (currentAge != userState.age)
            {
                userState.age = currentAge;
                userState.todaySessionsCount = 0;
                didChange = true;
            }

            return didChange;
        }

        public (SwJsonDictionary dataDictionary, IEnumerable<string> keysToEncrypt) ConditionallyAddExtraDataToTrackEvent(SwCoreUserData coreUserData, string eventName)
        {
            var extraData = new SwJsonDictionary
            {
                { MAIN_LEVEL_WISDOM_ANALYTICS_KEY, ImmutableUserState().lastLevelStarted },
                { TOTAL_LTV_GAME_SESSION_KEY, ImmutableUserState().gameSessionsCount },
                { TOTAL_LTV_GAME_FAILURE_KEY, ImmutableUserState().totalFailedLevels },
                { IS_TIME_BASED_ANALYTICS_KEY, Settings.isTimeBased.SwToInt() },
            };

            return (extraData, KeysToEncrypt());

            IEnumerable<string> KeysToEncrypt()
            {
                yield break;
            }
        }
        
        public IEnumerator OnDeepLinkResolved(Dictionary<string, string> deepLinkParams)
        {
            if (deepLinkParams.TryGetValue(SwStoreKeys.CurrentOrganizationAdvertisingId, out var orgId))
            {
                OrganizationAdvertisingId = orgId;
            }

            yield break;
        }
        
        #endregion


        #region --- Private Methods ---

        private void StartActiveDayProcess()
        {
            _activeDay.EnableFeature();
        }

        [NotNull]
        protected virtual SwUserState DeserializeUserState(string userStateString)
        {
            return SwUtils.JsonHandler.DeserializeObject<SwUserState>(userStateString) ?? new SwUserState();
        }

        // TODO Perry: We might want to include this logic inside the `ModifyUserStateSync` later, but currently we avoid it due to task complexity.
        internal void AfterUserStateChangeInternal(SwUserStateChangeReason reason, bool silent = false)
        {
            if (!silent)
            {
                OnUserStateChangeEvent?.Invoke(ImmutableUserState(), reason);
            }
        }

        internal SwUserState ImmutableUserState()
        {
            return UserState.Copy();
        }

        private long CalculateCurrentAgeDays()
        {
            return Convert.ToInt64((DateTime.UtcNow - InstallDateTime).Days);
        }

        private void InjectTestDataToUserState([NotNull] SwUserState userStateValue)
        {
            if (SwTestUtils.CustomUserState != null)
            {
                if (SwTestUtils.CustomUserState.ContainsKey("todaySessionsCount"))
                {
                    userStateValue.todaySessionsCount = (long)SwTestUtils.CustomUserState["todaySessionsCount"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("totalSessionsCount"))
                {
                    userStateValue.totalSessionsCount = (long)SwTestUtils.CustomUserState["totalSessionsCount"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("completedLevels"))
                {
                    userStateValue.completedLevels = (long)SwTestUtils.CustomUserState["completedLevels"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("playedLevels"))
                {
                    userStateValue.playedLevels = (long)SwTestUtils.CustomUserState["playedLevels"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("consecutiveFailedLevels"))
                {
                    userStateValue.consecutiveFailedLevels = (long)SwTestUtils.CustomUserState["consecutiveFailedLevels"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("consecutiveCompletedLevels"))
                {
                    userStateValue.consecutiveCompletedLevels = (long)SwTestUtils.CustomUserState["consecutiveCompletedLevels"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("isDuringLevel"))
                {
                    userStateValue.isDuringLevel = (bool)SwTestUtils.CustomUserState["isDuringLevel"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("totalFailedLevels"))
                {
                    userStateValue.totalFailedLevels = (long)SwTestUtils.CustomUserState["totalFailedLevels"];
                }
                
                if (SwTestUtils.CustomUserState.ContainsKey("gameSessionsCount"))
                {
                    userStateValue.gameSessionsCount = (long)SwTestUtils.CustomUserState["gameSessionsCount"];
                }
            }
        }

        private void LoadCustomUuid()
        {
            _customUuid = SwInfra.KeyValueStore.GetString(SwStoreKeys.CustomUuid);

            if (string.IsNullOrEmpty(_customUuid))
            {
                CustomUuid = Guid.NewGuid().ToString();
            }
        }

        private void LoadInstallDate()
        {
            var dateInSecondsFromPrefs = SwInfra.KeyValueStore.GetString(SwStoreKeys.InstallTime);
            var dateFromPrefs = SwInfra.KeyValueStore.GetString(SwStoreKeys.InstallDate);
            
            if (string.IsNullOrEmpty(InstallDateTimeString) && !string.IsNullOrEmpty(dateFromPrefs))
            {
                InstallDateTimeString = dateFromPrefs;
            }

            if (!string.IsNullOrEmpty(dateInSecondsFromPrefs))
            {
                try
                {
                    InstallDateInSeconds = Convert.ToInt64(dateInSecondsFromPrefs);
                }
                catch (Exception ex)
                {
                    SwInfra.Logger.LogException(ex, EWisdomLogType.User, "Cannot parse install in seconds from PlayerPerfs");
                }
            }

            // User is considered new if there are no saved values by wisdom
            IsNew = string.IsNullOrEmpty(InstallDateTimeString);
            
            var settings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Culture = CultureInfo.InvariantCulture,
            };

            if (IsNew)
            {
                InstallDateTime = DateTime.UtcNow;
                InstallDateTimeString = SwUtils.JsonHandler.SerializeObject(InstallDateTime, settings);
                
                var seconds = SwUtils.DateAndTime.GetTotalSeconds(InstallDateTime);
                
                if (InstallDateInSeconds == 0)
                {
                    InstallDateInSeconds = seconds;
                }
            }
            else
            {
                // Backward compatibility: in version 7.4 and lower -  Install date save in format yyyy-MM-dd 
                InstallDateTime = SwUtils.DateAndTime.TryParseDateTimeUtc(InstallDateTimeString, SwConstants.INSTALL_DATE_FORMAT) ?? SwUtils.JsonHandler.DeserializeObject<DateTime>(InstallDateTimeString, settings);
            }
        }

        protected void LoadUserState()
        {
            var userStateString = SwInfra.KeyValueStore.GetString(USER_STATE_STORAGE_KEY, "{}");
            var userStateValue = DeserializeUserState(userStateString);

            InjectTestDataToUserState(userStateValue);
            UpdateAge(userStateValue);
            _userState = userStateValue.Copy();
        }

        private void PersistUserState()
        {
            SwInfra.MainThreadRunner.RunOnMainThread(() =>
            {
            SwInfra.KeyValueStore.SetString(USER_STATE_STORAGE_KEY, SwUtils.JsonHandler.SerializeObject(ImmutableUserState()));
                SwInfra.KeyValueStore.Save();
            });
        }

        private void LoadActiveDays()
        {
            _activeDay.Load();
        }

        private void LoadInstallSdkVersion()
        {
            if (IsNew)
            {
                InstallSdkVersion = SwConstants.SDK_VERSION;
            }
            else
            {
                if (!SwInfra.KeyValueStore.HasKey(SwStoreKeys.InstallSdkVersion)) return;
                
                _installSdkVersion = SwInfra.KeyValueStore.GetString(SwStoreKeys.InstallSdkVersion);
            }
            
            InstallSdkVersionId = SwUtils.System.ComputeVersionId(InstallSdkVersion);
        }
        
        private void LoadInstallAppVersion()
        {
            if (IsNew)
            {
                InstallAppVersion = SwUtils.System.AppVersion;
            }
            else
            {
                if (!SwInfra.KeyValueStore.HasKey(SwStoreKeys.InstallAppVersion)) return;
                
                _installAppVersion = SwInfra.KeyValueStore.GetString(SwStoreKeys.InstallAppVersion);
            }
            
            InstallAppVersionId = SwUtils.System.ComputeVersionId(InstallAppVersion);
        }
        
#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
        internal void SetInstallSdkVersionForTest(string version)
        {
            InstallSdkVersionId = SwUtils.System.ComputeVersionId(version.SwToString());
            SwInfra.Logger.Log(EWisdomLogType.User, $"InstallSdkVersionId was set to: {InstallSdkVersionId}");
        }
#endif
        
        #endregion
    }
}