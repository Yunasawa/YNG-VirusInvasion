using YNL.Bases;

public class MiniQuest8 : BaseQuest
{
    public override void Initialize() => _target = 1;
    public override void Refresh(float current = 0) => Current = current;

    public override string GetProgress() => $"{Current}/{_target}";

    public override void OnAcceptQuest()
    {
        Initialize();

        if (!IsCompleted) Player.OnDefeatEnemy += OnDefeatEnemy;
    }

    public override void OnCompleteQuest()
    {
        if (!IsCompleted) Player.OnDefeatEnemy -= OnDefeatEnemy;
    }

    private void OnDefeatEnemy(string name)
    {
        if (name != "Boss 7 - 2") return;

        Current++;

        if (IsCompleted) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MiniQuest8", $"{Current}");
    }
}