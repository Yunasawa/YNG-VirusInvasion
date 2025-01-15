using YNL.Bases;

public class MiniQuest3 : BaseQuest
{
    public override void Initialize(float current = 0)
    {
        Current = current;
        _target = 20;
    }

    public override string GetProgress() => $"{Current}/{_target}";

    public override void OnAcceptQuest()
    {
        Initialize();

        if (!IsCompleted) Player.OnExchangeResources += OnExchangeResources;
    }

    public override void OnCompleteQuest()
    {
        Player.OnExchangeResources -= OnExchangeResources;
    }

    private void OnExchangeResources(string name, ResourcesInfo from, ResourcesInfo to)
    {
        if (name == "Exchanger 1") Current++;

        if (IsCompleted) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MiniQuest3", $"{Current}");
    }
}