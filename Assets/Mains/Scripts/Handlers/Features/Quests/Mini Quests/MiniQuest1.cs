using UnityEngine;

public class MiniQuest1 : BaseQuest
{
    public override void Initialize(float current = 0)
    {
        Current = current;
        _target = 100;
    }

    public override string GetProgress() => $"{Current}/{_target}";

    public override void OnAcceptQuest()
    {
        Initialize();

        if (!IsCompleted) Player.OnDefeatEnemy += OnDefeatEnemy;
    }

    public override void OnCompleteQuest()
    {
        Player.OnDefeatEnemy -= OnDefeatEnemy;
    }

    private void OnDefeatEnemy(string name)
    {
        Current++;

        if (IsCompleted) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MiniQuest1", $"{Current}");
    }
}