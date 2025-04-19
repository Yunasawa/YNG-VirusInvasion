using System;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwUserState
    {
        #region --- Public Methods ---

        public virtual SwUserState Copy()
        {
            return new SwUserState(this);
        }

        #endregion


        #region --- Members ---

        public long age;
        public long completedLevels;
        public long consecutiveCompletedLevels;
        public long consecutiveFailedLevels;
        public long totalFailedLevels;
        public long levelAttempts;
        public long levelRevives;
        public long playedLevels;
        public long todaySessionsCount;
        public long totalSessionsCount;
        public long gameSessionsCount;
        public bool isDuringLevel;
        public long lastLevelStarted;
        public long lastBonusLevelStarted;
        public long lastTutorialLevelStarted;
        public long completedBonusLevels;
        public long completedTutorialLevels;
        public long previousLevelTypeNumber;
        public ESwLevelType previousLevelType;
        public ESwLevelType currentLevelType;
        [CanBeNull] [NonSerialized] public string SessionId;

        #endregion


        #region --- Construction ---

        protected SwUserState(SwUserState other)
        {
            SessionId = other.SessionId;

            isDuringLevel = other.isDuringLevel;
            age = other.age;
            todaySessionsCount = other.todaySessionsCount;
            totalSessionsCount = other.totalSessionsCount;
            gameSessionsCount = other.gameSessionsCount;
            completedLevels = other.completedLevels;
            lastLevelStarted = other.lastLevelStarted;
            lastBonusLevelStarted = other.lastBonusLevelStarted;
            lastTutorialLevelStarted = other.lastTutorialLevelStarted;
            playedLevels = other.playedLevels;
            consecutiveFailedLevels = other.consecutiveFailedLevels;
            totalFailedLevels = other.totalFailedLevels;
            consecutiveCompletedLevels = other.consecutiveCompletedLevels;
            levelRevives = other.levelRevives;
            levelAttempts = other.levelAttempts;
            previousLevelType = other.previousLevelType;
            previousLevelTypeNumber = other.previousLevelTypeNumber;
            completedBonusLevels = other.completedBonusLevels;
            completedTutorialLevels = other.completedTutorialLevels;
            currentLevelType = other.currentLevelType;
        }

        internal SwUserState() { }

        #endregion
    }
}