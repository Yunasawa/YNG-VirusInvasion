using YNL.Bases;

public class MiniQuest2 : BaseQuest
{
    public override void Initialize(bool isCompleted, float current)
    {
        IsCompleted = isCompleted;
        Current = current;
        _target = 30;
    }

    public override string GetProgress() => $"{Current}/{_target}";

    public override void OnAcceptQuest()
    {
        Initialize(false, 0);

        if (!IsCompleted) Player.OnUpgradeAttribute += OnUpgradeAttribute;
    }

    public override void OnCompleteQuest()
    {
        if (!IsCompleted) Player.OnUpgradeAttribute -= OnUpgradeAttribute;
        IsCompleted = true;
    }

    private void OnUpgradeAttribute(AttributeType type)
    {
        Current++;

        if (Current >= _target) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MiniQuest2", $"{Current}");
    }
}