using UnityEngine;

public class MainQuest2 : BaseQuest
{
    public override void Initialize(float current = 0)
    {
        Current = current;
        _target = 8;
    }

    public override string GetProgress() => $"{Current}/{_target}";

    public override void OnAcceptQuest()
    {
        Initialize();

        OnCompleteQuest();

        if (!IsCompleted) Quest.OnCompleteQuest += OnCompleteQuest;
    }

    public override void OnCompleteQuest()
    {
        Quest.OnCompleteQuest -= OnCompleteQuest;
    }

    private void OnCompleteQuest(string key = "")
    {
        Current = 0;
        for (int i = 1; i <= 8; i++)
        {
            if (Game.Data.RuntimeQuestStats.CompletedQuests.Contains($"MiniQuest{i}")) Current++;
        }

        if (IsCompleted) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MainQuest2", $"{Current}");
    }
}