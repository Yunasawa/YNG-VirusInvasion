using YNL.Bases;

public class MiniQuest8 : BaseQuest
{
    public override void Initialize(float current = 0)
    {
        Current = current;
        _target = 30;
    }

    public override string GetProgress() => $"{Current}/{_target}";

    public override void OnAcceptQuest()
    {
        Initialize();

        //if (!IsCompleted) Player.OnTradeInMarket += OnTradeInMarket;
    }

    public override void OnCompleteQuest()
    {
        //if (!IsCompleted) Player.OnTradeInMarket -= OnTradeInMarket;
    }

    private void OnTradeInMarket(string name)
    {
        if (name == "Market 2") Current++;

        if (IsCompleted) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MiniQuest8", $"{Current}");
    }
}