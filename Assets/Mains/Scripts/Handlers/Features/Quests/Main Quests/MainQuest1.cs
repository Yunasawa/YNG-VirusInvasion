using UnityEngine;

public class MainQuest1 : BaseQuest
{
    public override void Initialize(bool isCompleted, float current)
    {
        IsCompleted = isCompleted;
        Current = current;
        _target = 2;
    }

    public override string GetProgress() => $"{Current}/{_target}";

    public override void OnAcceptQuest()
    {
        Initialize(false, 0);

        OnFarmStatsUpdate();

        if (!IsCompleted) Player.OnFarmStatsUpdate += OnFarmStatsUpdate;

        View.OnCloseQuestWindow?.Invoke();
        CameraManager.Instance.Door.FocusOnTarget(Game.Input.FoodFarm2.transform.position, 2, 5).Forget();
    }

    public override void OnCompleteQuest()
    {
        if (!IsCompleted) Player.OnFarmStatsUpdate -= OnFarmStatsUpdate;
        IsCompleted = true;
    }

    private void OnFarmStatsUpdate(string key = "")
    {
        foreach (var pair in Game.Data.RuntimeStats.ConstructStats.Farms)
        {
            Current = Mathf.Max(Current, pair.Value.Attributes["Income"].Level);
            Current = Mathf.Max(Current, pair.Value.Attributes["Capacity"].Level);
        }

        if (Current >= 2) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MainQuest1", $"{Current}");
    }
}