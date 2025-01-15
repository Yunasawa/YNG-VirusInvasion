using UnityEngine;

public class MainQuest1 : BaseQuest
{
    public override void Initialize() => _target = 2;
    public override void Refresh(float current = 0) => Current = current;

    public override string GetProgress() => $"{Current}/{_target}";

    public override void OnAcceptQuest()
    {
        Initialize();

        OnFarmStatsUpdate();

        if (!IsCompleted) Player.OnFarmStatsUpdate += OnFarmStatsUpdate;

        if (Game.IsFocusOnMainQuest1FirstTime)
        {
            View.OnCloseQuestWindow?.Invoke();
            CameraManager.Instance.Door.FocusOnTarget(Game.Input.FoodFarm2.transform.position, 2, 5).Forget();
            Game.IsFocusOnMainQuest1FirstTime = false;
        }

        Quest.OnProcessMainQuest1?.Invoke(true);
    }

    public override void OnCompleteQuest()
    {
        Player.OnFarmStatsUpdate -= OnFarmStatsUpdate;

        Quest.OnProcessMainQuest1?.Invoke(false);
    }

    private void OnFarmStatsUpdate(string key = "")
    {
        foreach (var pair in Game.Data.RuntimeStats.ConstructStats.Farms)
        {
            Current = Mathf.Max(Current, pair.Value.Attributes["Income"].Level);
            Current = Mathf.Max(Current, pair.Value.Attributes["Capacity"].Level);
        }

        if (IsCompleted) OnCompleteQuest();

        Quest.OnUpdateQuestStatus?.Invoke("MainQuest1", $"{Current}");
    }
}