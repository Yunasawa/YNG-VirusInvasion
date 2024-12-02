using System;

public static partial class View
{
    public static Action<ConstructType, bool, ConstructManager> OnOpenConstructWindow { get; set; }

    public static Action OnOpenResourceView { get; set; }
}

public enum ConstructType : byte { Base, Market, Exchanger, Farm, Quest }