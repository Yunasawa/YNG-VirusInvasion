using UnityEngine;
using YNL.Extensions.Methods;

public class QuestWindowUI : MonoBehaviour
{
    [SerializeField] private QuestFieldUI _stage2QuestUI;
    [SerializeField] private QuestFieldUI _stage3QuestUI;
    [SerializeField] private QuestFieldUI _stage4QuestUI;

    private void Awake()
    {
        Player.OnLevelUp += Start;

        Player.OnFarmStatsUpdate += OnUpgradeFarmAttribute;
    }

    private void OnDestroy()
    {
        Player.OnLevelUp -= Start;

        Player.OnFarmStatsUpdate -= OnUpgradeFarmAttribute;
    }

    private void Start()
    {
        _stage2QuestUI.Initialize(0);
        _stage3QuestUI.Initialize(0);
        _stage4QuestUI.Initialize(0);
    }

    private void OnUpgradeFarmAttribute(string key = "")
    {
        int higher = Mathf.Max(Game.Data.PlayerStats.FarmStats["Farm1.5"]["Income"].Level, Game.Data.PlayerStats.FarmStats["Farm1.5"]["Income"].Level);
        _stage2QuestUI.UpdateQuest(higher);
     
        if (higher >= 2) _stage2QuestUI.ShowReward();
    }
}