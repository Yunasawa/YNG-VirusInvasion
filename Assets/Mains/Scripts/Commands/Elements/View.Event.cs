using System;

public static partial class View
{
    public static Action<ConstructType, bool> OnOpenConstructWindow { get; set; }
}

public enum ConstructType : byte { Base, Market, Exchanger, Farm, Quest }