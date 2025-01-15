using YNL.Bases;

public class MiniQuest5 : BaseQuest
{
    public override void Initialize() => _target = 10000;
    public override void Refresh(float current = 0) => Current = current;

    public override string GetProgress() => $"{Current}/{_target}";

    public override void OnAcceptQuest()
    {
        Initialize();

        if (!IsCompleted) Player.OnCollectFarmResources += OnCollectFarmResources;
    }

    public override void OnCompleteQuest()
    {
        Player.OnCollectFarmResources -= OnCollectFarmResources;
    }

    private void OnCollectFarmResources(ResourceType type, float amount)
    {
        if (type == ResourceType.Food1) Current += amount;

        if (IsCompleted) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MiniQuest5", $"{Current}");
    }
}