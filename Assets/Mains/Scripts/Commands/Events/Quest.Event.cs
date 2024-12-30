using System;

public static partial class Quest
{
    public static Action<string, string> OnUpdateQuestStatus { get; set; }
}