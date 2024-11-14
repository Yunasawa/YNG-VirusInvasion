using System;

public static partial class View
{
    public static Action<ConstructType, bool> OnOpenConstructPanel { get; set; }
}

public enum ConstructType : byte { Base, Market, Exchanger, Farm, Quest }