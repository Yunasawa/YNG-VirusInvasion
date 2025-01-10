using UnityEngine;

public class MiniQuest1 : BaseQuest
{
    private int _defeatedAmount = 0;
    private int _targetAmount = 5;

    public override void Initialize(bool isCompleted, int target, int current)
    {
        IsCompleted = isCompleted;
        _targetAmount = target;
        _defeatedAmount = current;
    }

    public override string GetProgress() => $"{_defeatedAmount}/{_targetAmount}";

    public override (int, int) GetSerializeProgress() => new(_defeatedAmount, _targetAmount);

    public override void OnAcceptQuest()
    {
        if (!IsCompleted) Player.OnDefeatEnemy += OnDefeatEnemy;
    }

    public override void OnCompleteQuest()
    {
        if (!IsCompleted) Player.OnDefeatEnemy -= OnDefeatEnemy;
        IsCompleted = true;
    }

    private void OnDefeatEnemy(string name)
    {
        _defeatedAmount++;

        Quest.OnUpdateQuestStatus?.Invoke("MiniQuest1", $"{_defeatedAmount}");

        if (_defeatedAmount >= _targetAmount) OnCompleteQuest();
    }
}