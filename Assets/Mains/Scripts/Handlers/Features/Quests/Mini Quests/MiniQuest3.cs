using YNL.Bases;

public class MiniQuest3 : BaseQuest
{
    public override void Initialize(bool isCompleted, float current)
    {
        IsCompleted = isCompleted;
        Current = current;
        _target = 20;
    }

    public override string GetProgress() => $"{Current}/{_target}";

    public override void OnAcceptQuest()
    {
        Initialize(false, 0);

        if (!IsCompleted) Player.OnExchangeResources += OnExchangeResources;
    }

    public override void OnCompleteQuest()
    {
        if (!IsCompleted) Player.OnExchangeResources -= OnExchangeResources;
        IsCompleted = true;
    }

    private void OnExchangeResources(string name, ResourcesInfo from, ResourcesInfo to)
    {
        if (name == "Exchanger 1") Current++;

        if (Current >= _target) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MiniQuest3", $"{Current}");
    }
}