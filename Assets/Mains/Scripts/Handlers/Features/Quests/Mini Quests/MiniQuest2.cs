using YNL.Bases;

public class MiniQuest2 : BaseQuest
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

        if (!IsCompleted) Player.OnUpgradeAttribute += OnUpgradeAttribute;
    }

    public override void OnCompleteQuest()
    {
        Player.OnUpgradeAttribute -= OnUpgradeAttribute;
    }

    private void OnUpgradeAttribute(AttributeType type)
    {
        Current++;

        if (IsCompleted) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MiniQuest2", $"{Current}");
    }
}