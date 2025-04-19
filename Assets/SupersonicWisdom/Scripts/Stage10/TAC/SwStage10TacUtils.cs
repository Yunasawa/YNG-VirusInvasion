#if SW_STAGE_STAGE10_OR_ABOVE
namespace SupersonicWisdomSDK
{
    public static class SwStage10TacUtils
    {
        #region --- Private Methods ---

        internal static bool CheckAdditionalConditions(SwCoreTacConditions conditions, ISwRevenueCalculator revenueCalculator, SwUserActiveDay activeDay)
        {
            if (conditions == null)
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, "Conditions are null. Returning true.");

                return true;
            }

            if (conditions.MinRevenue.HasValue && conditions.MinRevenue.Value > revenueCalculator.Revenue)
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: MinRevenue is {conditions.MinRevenue.Value} but Revenue is {revenueCalculator.Revenue}");

                return false;
            }

            if (conditions.MinActiveDays.HasValue && conditions.MinActiveDays.Value > activeDay.ActiveDay)
            {
                SwInfra.Logger.Log(EWisdomLogType.TAC, $"Condition failed: MinActiveDays is {conditions.MinActiveDays.Value} but ActiveDay is {activeDay.ActiveDay}");

                return false;
            }

            SwInfra.Logger.Log(EWisdomLogType.TAC, "All additional conditions passed.");

            return true;
        }

        #endregion
    }
}
#endif