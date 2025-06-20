namespace SupersonicWisdomSDK
{
    internal struct SwGameStateProgressionData
    {
        //Reflection of values in UserState that are relevant to progression
        internal long? ConsecutiveCompletedLevels;
        internal long? ConsecutiveFailedLevels;
        internal long? TotalFailedLevels;
        internal long? LevelAttempts;
        internal long? LevelRevives;
        internal long? PlayedLevels;
        internal bool? IsDuringLevel;

        internal long? LastLevelStarted;
        internal long? LastBonusLevelStarted;
        internal long? LastTutorialLevelStarted;
        
        internal long? CompletedLevels;
        internal long? CompletedBonusLevels;
        internal long? CompletedTutorialLevels;
        internal long? PreviousLevelTypeNumber;
        internal ESwLevelType? PreviousLevelType;
        internal ESwLevelType? CurrentLevelType;

        public override string ToString()
        {
            return $"ConsecutiveCompletedLevels: {ConsecutiveCompletedLevels}, ConsecutiveFailedLevels: {ConsecutiveFailedLevels}, TotalFailedLevels: {TotalFailedLevels}, LevelAttempts: {LevelAttempts}, LevelRevives: {LevelRevives}, PlayedLevels: {PlayedLevels}, IsDuringLevel: {IsDuringLevel}, LastLevelStarted: {LastLevelStarted}, LastBonusLevelStarted: {LastBonusLevelStarted}, LastTutorialLevelStarted: {LastTutorialLevelStarted}, CompletedLevels: {CompletedLevels}, CompletedBonusLevels: {CompletedBonusLevels}, CompletedTutorialLevels: {CompletedTutorialLevels}, PreviousLevelTypeNumber: {PreviousLevelTypeNumber}, PreviousLevelType: {PreviousLevelType}, CurrentLevelType: {CurrentLevelType}";
        }
    }
}