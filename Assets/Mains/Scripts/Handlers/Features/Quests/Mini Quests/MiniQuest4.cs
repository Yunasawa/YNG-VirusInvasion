using YNL.Bases;

public class MiniQuest4 : BaseQuest
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

        if (!IsCompleted) Player.OnTradeInMarket += OnTradeInMarket;
    }

    public override void OnCompleteQuest()
    {
        Player.OnTradeInMarket -= OnTradeInMarket;
    }

    private void OnTradeInMarket(string name)
    {
        if (name == "Market 2") Current++;

        if (IsCompleted) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MiniQuest4", $"{Current}");
    }
}