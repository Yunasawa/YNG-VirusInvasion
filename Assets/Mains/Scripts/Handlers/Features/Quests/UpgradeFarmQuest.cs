public class UpgradeFarmQuest : Quest
{
    public override void OnAcceptQuest()
    {
        Player.OnFarmStatsUpdate += OnFarmStatsUpdate;
    }

    public override void OnCompleteQuest()
    {
        Player.OnFarmStatsUpdate -= OnFarmStatsUpdate;
        
    }

    private void OnFarmStatsUpdate(string key = "")
    {

    }
}