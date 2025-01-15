using YNL.Bases;

public class MiniQuest7 : BaseQuest
{
    public override void Initialize(float current = 0)
    {
        Current = current;
        _target = 1000;
    }

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
        if (type == ResourceType.Gen1) Current += amount;

        if (IsCompleted) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MiniQuest7", $"{Current}");
    }
}