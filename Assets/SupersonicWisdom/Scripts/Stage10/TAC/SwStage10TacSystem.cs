#if SW_STAGE_STAGE10_OR_ABOVE

namespace SupersonicWisdomSDK
{
    internal class SwStage10TacSystem : SwTacSystem, ISwLocalConfigProvider
    {
        #region --- Members ---

        private ISwRevenueCalculator _revenueCalculator;
        private SwUserActiveDay _activeDay;

        #endregion


        #region --- Public Methods ---

        public void InjectDependencies(SwCoreUserData userData, SwTimerManager timerManager, SwGameStateSystem gameStateSystem, SwSettings settings, SwUserActiveDay activeDay, ISwRevenueCalculator revenueCalculator)
        {
            base.InjectDependencies(userData, timerManager, gameStateSystem, settings);

            _activeDay = activeDay;
            _revenueCalculator = revenueCalculator;
        }
        
        public virtual SwLocalConfig GetLocalConfig()
        {
            return new SwStage10TacLocalConfig();
        }

        #endregion


        #region --- Private Methods ---

        protected override bool CheckCondition(SwCoreTacConditions conditions)
        {
            return base.CheckCondition(conditions)
                   && SwStage10TacUtils.CheckAdditionalConditions(conditions, _revenueCalculator, _activeDay);
        }

        #endregion
    }
}

#endif