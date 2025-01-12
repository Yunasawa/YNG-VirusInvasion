using System;

public static partial class Quest
{
    public static Action<string, string> OnUpdateQuestStatus { get; set; }
    public static Action<string> OnDeserializeQuest { get; set; }
    public static Action<string> OnCompleteQuest { get; set; }

    public static Action<bool> OnProcessMainQuest1 { get; set; }
}