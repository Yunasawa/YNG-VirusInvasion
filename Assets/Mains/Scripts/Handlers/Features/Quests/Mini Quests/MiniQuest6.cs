using YNL.Bases;

public class MiniQuest6 : BaseQuest
{
    public override void Initialize(bool isCompleted, float current)
    {
        IsCompleted = isCompleted;
        Current = current;
        _target = 200;
    }

    public override string GetProgress() => $"{Current}/{_target}";

    public override void OnAcceptQuest()
    {
        Initialize(false, 0);

        if (!IsCompleted) Player.OnCollectEnemyDrops += OnCollectEnemyDrops;
    }

    public override void OnCompleteQuest()
    {
        if (!IsCompleted) Player.OnCollectEnemyDrops -= OnCollectEnemyDrops;
        IsCompleted = true;
    }

    private void OnCollectEnemyDrops((ResourceType Type, uint Amount)[] drops)
    {
        foreach (var drop in drops)
        {
            if (drop.Type == ResourceType.Energy3 && Player.Stage.CurrentStage == StageType.Stage6)
            {
                Current += drop.Amount;
            }
        }

        if (Current >= _target) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MiniQuest6", $"{Current}");
    }
}