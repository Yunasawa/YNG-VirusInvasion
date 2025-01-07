using UnityEngine;

public class MiniQuest1 : BaseQuest
{
    private int _defeatedAmount = 0;

    public override string GetProgress()
    {
        return $"{_defeatedAmount}/50";
    }

    public override void OnAcceptQuest()
    {
        Player.OnDefeatEnemy += OnDefeatEnemy;
    }

    public override void OnCompleteQuest()
    {
        Player.OnDefeatEnemy -= OnDefeatEnemy;
    }

    private void OnDefeatEnemy(string name)
    {
        _defeatedAmount++;

        Quest.OnUpdateQuestStatus?.Invoke("MiniQuest1", $"{_defeatedAmount}");

        if (_defeatedAmount >= 50) OnCompleteQuest();
    }
}