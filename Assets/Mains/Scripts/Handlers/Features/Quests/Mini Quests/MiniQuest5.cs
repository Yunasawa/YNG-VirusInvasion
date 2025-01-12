using YNL.Bases;

public class MiniQuest5 : BaseQuest
{
    public override void Initialize(bool isCompleted, float current)
    {
        IsCompleted = isCompleted;
        Current = current;
        _target = 10000;
    }

    public override string GetProgress() => $"{Current}/{_target}";

    public override void OnAcceptQuest()
    {
        Initialize(false, 0);

        if (!IsCompleted) Player.OnCollectFarmResources += OnCollectFarmResources;
    }

    public override void OnCompleteQuest()
    {
        if (!IsCompleted) Player.OnCollectFarmResources -= OnCollectFarmResources;
        IsCompleted = true;
    }

    private void OnCollectFarmResources(ResourceType type, float amount)
    {
        if (type == ResourceType.Food1) Current += amount;

        if (Current >= _target) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MiniQuest5", $"{Current}");
    }
}