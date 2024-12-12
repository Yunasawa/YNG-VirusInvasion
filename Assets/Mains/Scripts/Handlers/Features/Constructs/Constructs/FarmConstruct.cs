using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using YNL.Bases;

public class FarmConstruct : MonoBehaviour
{
    private DateTime _previousTime;
    private DateTime _currentTime;

    private ConstructManager _manager;

    [SerializeField] private GameObject _exclamationMark;

    public float CurrentResources;
    public int Capacity => Mathf.RoundToInt(Game.Data.PlayerStats.FarmStats[_manager.Name]["Capacity"].Value);

    public ResourceType GeneratedResource;

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
    }

    private void Update()
    {
        Vector3 direction = Camera.main.transform.forward;
        Quaternion rotation = Quaternion.LookRotation(direction);

        //_canvas.transform.rotation = rotation;
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

    public void StartCountToPing(int second)
    {
        CountToPingTask(second).Forget();
    }

    private async UniTask CountToPingTask(int second)
    {
        await UniTask.WaitForSeconds(second);
        //ResourcePing.SetActive(true);
    }
}