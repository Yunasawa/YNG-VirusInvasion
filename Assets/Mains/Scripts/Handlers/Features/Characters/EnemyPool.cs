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
    [SerializeField] private string _enemyName;
    private EnemySources _enemySources;
    [SerializeField] private float _boundaryRadius = 10;
    private int _enemyCount = 0;

    private Boundary _boundary => Game.Data.BoundaryStages[_stageType];

    private Enemy[] _enemies;
    private List<float> _respawnCounter = new();
    private ObjectPool<Enemy> _enemyPool;

    private TransformAccessArray _enemyTransforms;
    private NativeArray<float3> _targetPositions;
    private NativeArray<bool> _isPulling;
    private NativeArray<float> _movingSpeed;

    private void Start()
    {
        _enemySources = Game.Data.EnemySources[_enemyName];

        _enemyCount = _enemySources.EnemyAmount;

        _enemies = new Enemy[_enemyCount];
        _enemyPool = new(_enemySources.Enemy.gameObject, this.transform);

        _enemyTransforms = new TransformAccessArray(_enemyCount);
        _targetPositions = new NativeArray<float3>(_enemyCount, Allocator.Persistent);
        _isPulling = new NativeArray<bool>(_enemyCount, Allocator.Persistent);
        _movingSpeed = new NativeArray<float>(_enemyCount, Allocator.Persistent);

        for (int i = 0; i < _enemyCount; i++)
        {
            Vector3 spawnPosition = GetRandomPositionInsideCircle();
            _enemies[i] = _enemyPool.PullLocalObject(spawnPosition, Quaternion.identity);
            _enemies[i].Pool = this;
            _enemyTransforms.Add(_enemies[i].transform);
            _targetPositions[i] = GetRandomPositionInsideCircle();

            _enemies[i].transform.localPosition = spawnPosition;
        }
    }

    private void Update()
    {
        for (int i = 0; i < _enemyCount; i++)
        {
            if (math.length(_targetPositions[i] - (float3)_enemyTransforms[i].localPosition) < 0.2f)
            {
                _targetPositions[i] = GetRandomPositionInsideCircle();
            }
            else if (math.length(transform.position - _enemyTransforms[i].position) > _boundaryRadius)
            {
                _targetPositions[i] = GetRandomPositionInsideCircle();
            }

            _isPulling[i] = _enemies[i].Movement.IsPulling;
            _movingSpeed[i] = _enemies[i].IsCaught ? 5 : 1.5f;
        }

        UpdateEnemyMovements();

        for (int i = 0; i < _enemyPool.activeObjects.Count; i++) _enemyPool.activeObjects[i].MonoUpdate();

        SpawnOnCountingEnd();
    }

    private void OnDestroy()
    {
        if (_targetPositions.IsCreated) _targetPositions.Dispose();
        if (_enemyTransforms.isCreated) _enemyTransforms.Dispose();
        if (_isPulling.IsCreated) _isPulling.Dispose();
        if (_movingSpeed.IsCreated) _movingSpeed.Dispose();
    }

    public void PullObject() => _enemyPool.PullLocalObject(GetRandomPositionInsideCircle(), Quaternion.identity);

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
            DeltaTime = Time.deltaTime,
            PlayerPosition = Player.Transform.position,
            IsPulling = _isPulling,
            MovingSpeed = _movingSpeed
        };

        JobHandle handle = job.Schedule(_enemyTransforms);
        handle.Complete();
    }

    public void NotifyOnRespawn()
    {
        _respawnCounter.Add(_enemySources.RespawnTime);

        if (_enemyPool.activeObjects.Count <= 0) this.transform.position = _boundary.GetRandomPositionInRandomBoundary();
    }

    private void SpawnOnCountingEnd()
    {
        for (int i = 0; i < _respawnCounter.Count; i++) _respawnCounter[i] -= Time.deltaTime;

        for (int i = _respawnCounter.Count - 1; i >= 0; i--)
        {
            if (_respawnCounter[i] <= 0)
            {
                PullObject();
                _respawnCounter.RemoveAt(i);
            }
        }
    }
}

[BurstCompile]
public struct EnemyMovementJob : IJobParallelForTransform
{
    public NativeArray<float3> TargetPositions;
    public float PullingSpeed;
    public float DeltaTime;
    public float3 PlayerPosition;
    public NativeArray<bool> IsPulling;
    public NativeArray<float> MovingSpeed;

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
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, MovingSpeed[index] * DeltaTime * 2);

                transform.localPosition += transform.rotation * Vector3.forward * MovingSpeed[index] * DeltaTime;
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
