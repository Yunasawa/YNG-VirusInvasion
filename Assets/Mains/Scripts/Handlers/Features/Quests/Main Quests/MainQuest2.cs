using UnityEngine;

public class MainQuest2 : BaseQuest
{
    public override void Initialize(bool isCompleted, float current)
    {
        IsCompleted = isCompleted;
        Current = current;
        _target = 8;
    }

    public override string GetProgress() => $"{Current}/{_target}";

    public override void OnAcceptQuest()
    {
        Initialize(false, 0);

        OnCompleteQuest();

        if (!IsCompleted) Quest.OnCompleteQuest += OnCompleteQuest;
    }

    public override void OnCompleteQuest()
    {
        if (!IsCompleted) Quest.OnCompleteQuest -= OnCompleteQuest;
        IsCompleted = true;
    }

    private void OnCompleteQuest(string key = "")
    {
        Current = 0;
        for (int i = 1; i <= 8; i++)
        {
            if (Game.Data.RuntimeQuestStats.CompletedQuests.Contains($"MiniQuest{i}")) Current++;
        }

        if (Current >= 8) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MainQuest2", $"{Current}");
    }
}