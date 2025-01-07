using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YNL.Extensions.Methods;

public class EnemyPoolManager : MonoBehaviour
{
    [SerializeField] private StageType _stageType;
    [SerializeField] private List<EnemyPool> _enemyPools = new();

    private void Awake()
    {
        Player.OnEnterStage += OnEnterStage;
    }

    private void OnDestroy()
    {
        Player.OnEnterStage -= OnEnterStage;
    }

    private void Start()
    {
        StartAsync().Forget();

        async UniTaskVoid StartAsync()
        {
            await UniTask.NextFrame();

            if (_stageType != Player.Stage.CurrentStage)
            {
                foreach (var pool in _enemyPools) pool.gameObject.SetActive(false);
            }
        }
    }

    [Button]
    public void RefreshPools()
    {
        _enemyPools.Clear();

        foreach (var pool in this.transform.Cast<Transform>())
        {
            _enemyPools.Add(pool.GetComponent<EnemyPool>());
        }
    }

    private void OnEnterStage(StageType previous, StageType current)
    {
        foreach (var pool in _enemyPools)
        {
            if (_stageType == previous) pool.gameObject.SetActive(false);
            else if (_stageType == current) pool.gameObject.SetActive(true);
        }
    }
}