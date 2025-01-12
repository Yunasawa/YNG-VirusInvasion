using Cysharp.Threading.Tasks;
using System;
using Unity.Burst.Intrinsics;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;

public class FarmConstruct : MonoBehaviour
{
    private RuntimeConstructStats _runtimeConstructStats => Game.Data.RuntimeStats.ConstructStats;
    private RuntimeFarmStats _farmStats => _runtimeConstructStats.Farms[Manager.Name];

    public ConstructManager Manager;

    [SerializeField] private GameObject _exclamationMark;
    [SerializeField] private FarmBillboardUI _ui; 

    public int Capacity => Mathf.RoundToInt(_runtimeConstructStats.Farms[Manager.Name].Attributes["Capacity"].Value);

    public ResourceType GeneratedResource;
    public int TimeCounter = Key.Config.FarmCountdown;

    private void Awake()
    {
        Manager = GetComponent<ConstructManager>();

        _ui.OnCollectResource = CollectResource;

        Quest.OnProcessMainQuest1 += OnProcessMainQuest1;
    }

    private void OnDestroy()
    {
        Quest.OnProcessMainQuest1 -= OnProcessMainQuest1;
    }

    private void Start()
    {
        _ui.Initialize(Manager.Name, GeneratedResource);
        _ui.UpdateUI(_farmStats.Current, Capacity);

        GenerateResource().Forget();
    }

    private void OnProcessMainQuest1(bool isProcess)
    {
        _exclamationMark.SetActive(isProcess);
    }

    public void CollectResource()
    {
        Player.OnCollectFarmResources?.Invoke(GeneratedResource, _farmStats.Current);
        _farmStats.Current = 0;

        UpdateUI();
    }

    public void UpdateUI()
    {
        _ui.UpdateUI(_farmStats.Current, Capacity);
    }

    private async UniTask GenerateResource()
    {
        while (true)
        {
            await UniTask.WaitForSeconds(1);
            TimeCounter--;

            if (TimeCounter < 0)
            {
                TimeCounter = Key.Config.FarmCountdown;

                if (_farmStats.Current >= Capacity) continue;

                _farmStats.Current += _farmStats.Attributes["Income"].Value;
                _farmStats.Current = Mathf.Min(_farmStats.Current, Capacity);
                UpdateUI();

                Construct.OnFarmGenerateResource?.Invoke(this);
            }

            Construct.OnFarmCountdown?.Invoke(this, TimeCounter);
        }
    }
}