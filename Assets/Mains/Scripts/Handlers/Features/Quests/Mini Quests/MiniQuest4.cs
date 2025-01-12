using YNL.Bases;

public class MiniQuest4 : BaseQuest
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

        if (!IsCompleted) Player.OnTradeInMarket += OnTradeInMarket;
    }

    public override void OnCompleteQuest()
    {
        if (!IsCompleted) Player.OnTradeInMarket -= OnTradeInMarket;
        IsCompleted = true;
    }

    private void OnTradeInMarket(string name)
    {
        if (name == "Market 2") Current++;

        if (Current >= _target) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MiniQuest4", $"{Current}");
    }
}