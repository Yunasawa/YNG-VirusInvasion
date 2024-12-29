using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;

public class FarmConstruct : MonoBehaviour
{
    public ConstructManager Manager;

    [SerializeField] private GameObject _exclamationMark;
    [SerializeField] private FarmBillboardUI _ui; 

    public float CurrentResources;
    public int Capacity => Mathf.RoundToInt(Game.Data.PlayerStats.FarmStats[Manager.Name]["Capacity"].Value);

    public ResourceType GeneratedResource;
    private int _timeCounter = Key.Config.FarmCountdown;

    private void Awake()
    {
        Manager = GetComponent<ConstructManager>();

        Player.OnAcceptQuest += OnAcceptQuest;
        Player.OnFinishQuest += OnFinishQuest;
    }

    private void OnDestroy()
    {
        Player.OnAcceptQuest -= OnAcceptQuest;
        Player.OnFinishQuest -= OnFinishQuest;
    }

    private void Start()
    {
        _ui.Initialize(Manager.Name, GeneratedResource);
        _ui.UpdateUI(CurrentResources, Capacity);

        GenerateResource().Forget();
    }

    private void OnAcceptQuest(string name)
    {
        if (name != "UpgradeFarm") return;

        _exclamationMark.SetActive(true);
    }

    private void OnFinishQuest(string name)
    {
        if (name != "UpgradeFarm") return;

        _exclamationMark.SetActive(false);
    }

    private async UniTask GenerateResource()
    {
        while (true)
        {
            await UniTask.WaitForSeconds(1);
            _timeCounter--;

            if (_timeCounter < 0)
            {
                _timeCounter = Key.Config.FarmCountdown;

                if (CurrentResources >= Capacity) return;

                CurrentResources += Game.Data.PlayerStats.FarmStats[Manager.Name]["Income"].Value;
                _ui.UpdateUI(CurrentResources, Capacity);

                Construct.OnFarmGenerateResource?.Invoke(this);
            }

            Construct.OnFarmCountdown?.Invoke(this, _timeCounter);
        }
    }
}