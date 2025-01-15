using YNL.Bases;

public class MiniQuest6 : BaseQuest
{
    public override void Initialize() => _target = 200;
    public override void Refresh(float current = 0) => Current = current;

    public override string GetProgress() => $"{Current}/{_target}";

    public override void OnAcceptQuest()
    {
        Initialize();

        if (!IsCompleted) Player.OnCollectEnemyDrops += OnCollectEnemyDrops;
    }

    public override void OnCompleteQuest()
    {
        Player.OnCollectEnemyDrops -= OnCollectEnemyDrops;
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

        if (IsCompleted) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MiniQuest6", $"{Current}");
    }
}