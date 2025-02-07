using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using YNL.Extensions.Methods;

public class SMPool : MonoBehaviour
{
    private RuntimeEnemyStats _runtimeEnemyStats => Game.Data.RuntimeEnemy.EnemyStats;

    [SerializeField] private TextMeshPro _text;

    [SerializeField] private string _smName;
    private EnemyPool _pool;

    private void Awake()
    {
        _pool = GetComponent<EnemyPool>();

        _pool.OnWaitToRespawn += OnWaitToRespawn;
    }

    private void OnDestroy()
    {
        _pool.OnWaitToRespawn -= OnWaitToRespawn;
    }

    private void Start()
    {
        EnemyRespawnTimeSpan timeSpan = _runtimeEnemyStats.SMVirusRespawnTimes[_smName];
        TimeSpan passedTime = DateTime.Now - timeSpan.DateTime;
        timeSpan.Time -= passedTime.Seconds;
        if (timeSpan.Time > 0)
        {
            OnWaitToRespawn((uint)timeSpan.Time);
            _pool.UpdateForSMVirus(timeSpan.Time).Forget();
        }
    }

    private void OnWaitToRespawn(uint time)
    {
        WaitToRespawn(time - 1).Forget();
    }

    private async UniTask WaitToRespawn(uint time)
    {
        _text.gameObject.SetActive(true);

        while (time > 0)
        {
            string formattedTime = TimeSpan.FromSeconds(time).ToString(@"hh\:mm\:ss");
            _text.text = $"Next unit\n{formattedTime}";

            time--;
            _runtimeEnemyStats.SMVirusRespawnTimes[_smName].Time = (int)time;
            _runtimeEnemyStats.SMVirusRespawnTimes[_smName].DateTime = DateTime.Now;

            await UniTask.WaitForSeconds(1);
        }

        _text.gameObject.SetActive(false);
    }
}