using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;

public class FarmConstruct : MonoBehaviour
{
    private DateTime _previousTime;
    private DateTime _currentTime;

    private ConstructManager _manager;

    [SerializeField] private GameObject _exclamationMark;
    [SerializeField] private FarmBillboardUI _ui; 

    public float CurrentResources;
    public int Capacity => Mathf.RoundToInt(Game.Data.PlayerStats.FarmStats[_manager.Name]["Capacity"].Value);

    public ResourceType GeneratedResource;
    public bool IsWindowOpening = false;

    private void Awake()
    {
        _manager = GetComponent<ConstructManager>();

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
        _previousTime = DateTime.Now;

        _ui.Initialize(_manager.Name, GeneratedResource);
    }

    private void Update()
    {
        Vector3 direction = Camera.main.transform.forward;
        Quaternion rotation = Quaternion.LookRotation(direction);
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

    public int DeltaSecond()
    {
        _currentTime = DateTime.Now;
        TimeSpan delta = _currentTime - _previousTime;
        return (int)delta.TotalSeconds;
    }

    public void OnWindowClose()
    {
        _previousTime = DateTime.Now;
    }

    public async UniTask CountToPingTask(int second)
    {
        await UniTask.WaitForSeconds(second);
        StartGenerateResource().Forget();
    }

    private async UniTask StartGenerateResource()
    {
        while (!IsWindowOpening)
        {
            await UniTask.WaitForSeconds(Key.Config.FarmCountdown);

            if (CurrentResources >= Capacity) continue;

            CurrentResources += Game.Data.PlayerStats.FarmStats[_manager.Name]["Income"].Value;
            _ui.UpdateUI(CurrentResources, Capacity);
        }
    }
}