using System;

public static partial class View
{
    public static Action<ConstructType, bool, ConstructManager> OnOpenConstructWindow { get; set; }

    public static Action OnOpenResourceView { get; set; }
    public static Action OnChangeLevelField { get; set; }

    public static Action OnCloseUpgradeWindow { get; set; }
    public static Action OnCloseQuestWindow { get; set; }

    public static Action<SettingsType> OnToggleSettings { get; set; }
}

public enum ConstructType : byte { Base, Market, Exchanger, Farm, Quest }
public enum SettingsType : byte { Sound, Music }