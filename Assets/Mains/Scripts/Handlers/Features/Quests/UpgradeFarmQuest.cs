using UnityEngine;

public class UpgradeFarmQuest : BaseQuest
{
    private RuntimeConstructStats _runtimeConstructStats => Game.Data.RuntimeStats.ConstructStats;

    private int _highestLevel = 0;

    public override void OnAcceptQuest()
    {
        OnFarmStatsUpdate();

        if (!IsCompleted) Player.OnFarmStatsUpdate += OnFarmStatsUpdate;
    }

    public override string GetProgress() => $"{_highestLevel}/2";

    public override void OnCompleteQuest()
    {
        if (!IsCompleted) Player.OnFarmStatsUpdate -= OnFarmStatsUpdate;
        IsCompleted = true;
    }

    private void OnFarmStatsUpdate(string key = "")
    {
        foreach (var pair in Game.Data.RuntimeStats.ConstructStats.Farms)
        {
            _highestLevel = Mathf.Max(_highestLevel, pair.Value.Attributes["Income"].Level);
            _highestLevel = Mathf.Max(_highestLevel, pair.Value.Attributes["Capacity"].Level);
        }

        if (_highestLevel >= 2)
        {
            OnCompleteQuest();
            Quest.OnUpdateQuestStatus?.Invoke("UpgradeFarm", $"{_highestLevel}");
        }
    }
}