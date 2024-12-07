using OWS.ObjectPooling;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using YNL.Bases;
using YNL.Extensions.Methods;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private StageType _stageType;
    [SerializeField] private Dictionary<string, EnemySources> _enemySources;
    [SerializeField] private float _boundaryRadius = 10;
    private int _enemyCount = 0;

    private Enemy[] _enemies;
    private Dictionary<string, ObjectPool<Enemy>> _enemyPools = new();

    private TransformAccessArray _enemyTransforms;
    private NativeArray<float3> _targetPositions;
    private NativeArray<bool> _isPulling;

    private void Start()
    {
        _enemyCount = 15;

        if (Game.Data.EnemyStages.TryGetValue(_stageType, out string sources))
        {
            _enemySources = sources.Split(new[] { ", " }, System.StringSplitOptions.RemoveEmptyEntries)
                .Where(name => Game.Data.EnemySources.TryGetValue(name, out _))
                .ToDictionary(name => name, name => Game.Data.EnemySources[name]);
        }

        foreach (var pair in _enemySources) _enemyPools.Add(pair.Key, new(pair.Value.Enemy.gameObject, this.transform));

        _enemies = new Enemy[_enemyCount];

        _enemyTransforms = new TransformAccessArray(_enemyCount);
        _targetPositions = new NativeArray<float3>(_enemyCount, Allocator.Persistent);
        _isPulling = new NativeArray<bool>(_enemyCount, Allocator.Persistent);

        for (int i = 0; i < _enemyCount; i++)
        {
            Vector3 spawnPosition = GetRandomPositionInsideCircle();
            _enemies[i] = _enemyPools[GetRandomEnemySource()].PullLocalObject(spawnPosition, Quaternion.identity);
            _enemyTransforms.Add(_enemies[i].transform);
            _targetPositions[i] = GetRandomPositionInsideCircle();

            _enemies[i].transform.localPosition = spawnPosition;
        }
    }

    private void Update()
    {
        for (int i = 0; i < _enemyCount; i++)
        {
            if (math.length(_targetPositions[i] - (float3)_enemyTransforms[i].localPosition) < 0.1f)
            {
                _targetPositions[i] = GetRandomPositionInsideCircle();
            }
            else if (math.length(transform.position - _enemyTransforms[i].localPosition) > _boundaryRadius)
            {
                _targetPositions[i] = GetRandomPositionInsideCircle();
            }


            _isPulling[i] = _enemies[i].Movement.IsPulling;
        }

        UpdateEnemyMovements();

        foreach (var pool in _enemyPools)
        {
            for (int i = 0; i < pool.Value.activeObjects.Count; i++) pool.Value.activeObjects[i].MonoUpdate();
        }
    }

    private void OnDestroy()
    {
        _targetPositions.Dispose();
        _enemyTransforms.Dispose();
        _isPulling.Dispose();
    }

    private string GetRandomEnemySource()
    {
        var totalWeight = _enemySources.Sum(s => s.Value.RespawnTime);
        var randomValue = UnityEngine.Random.Range(0, totalWeight);

        long cumulativeWeight = 0;

        foreach (var source in _enemySources)
        {
            cumulativeWeight += source.Value.RespawnTime;
            if (randomValue < cumulativeWeight) return source.Key;
        }
        return null;
    }

    public void PullObject() => _enemyPools[GetRandomEnemySource()].PullLocalObject(GetRandomPositionInsideCircle(), Quaternion.identity);

    private float3 GetRandomPositionInsideCircle()
    {
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);
        float distance = UnityEngine.Random.Range(0f, _boundaryRadius);
        return new float3(math.cos(angle) * distance, 0, math.sin(angle) * distance);
    }

    private void UpdateEnemyMovements()
    {
        var job = new EnemyMovementJob
        {
            TargetPositions = _targetPositions,
            PullingSpeed = Player.Movement.CurrentMovingSpeed.Oscillate() * Formula.Value.GetEnemyPullingSpeedMultiplier(transform.position),
            MovingSpeed = 1.5f,
            DeltaTime = Time.deltaTime,
            PlayerPosition = Player.Transform.position,
            IsPulling = _isPulling
        };

        JobHandle handle = job.Schedule(_enemyTransforms);
        handle.Complete();
    }
}

[BurstCompile]
public struct EnemyMovementJob : IJobParallelForTransform
{
    public NativeArray<float3> TargetPositions;
    public float PullingSpeed;
    public float MovingSpeed;
    public float DeltaTime;
    public float3 PlayerPosition;
    public NativeArray<bool> IsPulling;

    public void Execute(int index, TransformAccess transform)
    {
        if (IsPulling[index])
        {
            transform.position = Vector3.MoveTowards(transform.position, PlayerPosition, PullingSpeed);
        }
        else
        {
            float3 direction = math.normalize(TargetPositions[index] - (float3)transform.localPosition);
            if (!math.all(direction == float3.zero))
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, MovingSpeed * DeltaTime * 2);

                transform.localPosition += transform.rotation * Vector3.forward * MovingSpeed * DeltaTime;
            }
        }
    }
}

[System.Serializable]
public struct Bound
{
    public Vector3 TopFrontLeft;
    public Vector3 BottomBackRight;
}
