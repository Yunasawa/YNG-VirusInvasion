using YNL.Bases;
using YNL.Utilities.Addons;

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

    private void OnCollectEnemyDrops(int capacity, SerializableDictionary<ResourceType, uint> drops)
    {
        foreach (var drop in drops)
        {
            if (drop.Key == ResourceType.Energy3 && Player.Stage.CurrentStage == StageType.Stage6)
            {
                Current += drop.Value;
            }
        }

        if (IsCompleted) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MiniQuest6", $"{Current}");
    }
}