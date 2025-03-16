using System;

public static partial class Game
{
    public static Action OnStart { get; set; }

    public static bool IsStartGameTutorialActivated = false;
    public static bool IsReturnToBaseTutorialActivated = false;
    public static bool IsUpgradeAttributeTutorialActivated = false;

    public static bool IsFocusOnMainQuest1FirstTime = true;
    public static bool IsUnlockedDoorStage6 = false;

    public static bool IsPolicyChecked = false;
}