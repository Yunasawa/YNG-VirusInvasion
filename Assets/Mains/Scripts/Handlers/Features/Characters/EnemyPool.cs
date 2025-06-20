using Cysharp.Threading.Tasks;
using OWS.ObjectPooling;
using System;
using System.Collections.Generic;
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
    public string EnemyName => _enemyName;
    private EnemySources _enemySources;
    [SerializeField] private float _boundaryRadius = 10;
    private int _enemyCount = 0;
    [SerializeField] private bool _moveWhenClear = true;
    [SerializeField] private bool _enableMove = true;

    private Boundary _boundary => Game.Data.BoundaryStages[_stageType];

    private Enemy[] _enemies;
    public Enemy[] Enemies => _enemies;
    private List<float> _respawnCounter = new();
    private ObjectPool<Enemy> _enemyPool;

    private TransformAccessArray _enemyTransforms;
    private NativeArray<float3> _targetPositions;
    private NativeArray<float3> _playerPositions;
    private NativeArray<bool> _isPulling;
    private NativeArray<float> _movingSpeed;

    public Action<uint> OnWaitToRespawn;

    private void OnValidate()
    {
        this.gameObject.name = _enemyName;
    }

    private void Start()
    {
        Game.Enemy.RegisterEnemyPool(this);

        _enemySources = Game.Data.EnemySources[_enemyName];

        _enemyCount = _enemySources.EnemyAmount;

        _enemies = new Enemy[_enemyCount];
        _enemyPool = new(_enemySources.Enemy.gameObject, this.transform);

        _enemyTransforms = new TransformAccessArray(_enemyCount);
        _targetPositions = new NativeArray<float3>(_enemyCount, Allocator.Persistent);
        _playerPositions = new NativeArray<float3>(_enemyCount, Allocator.Persistent);
        _isPulling = new NativeArray<bool>(_enemyCount, Allocator.Persistent);
        _movingSpeed = new NativeArray<float>(_enemyCount, Allocator.Persistent);

        for (int i = 0; i < _enemyCount; i++)
        {
            Vector3 spawnPosition = GetRandomPositionInsideCircle();
            _enemies[i] = _enemyPool.PullLocalObject(spawnPosition, Quaternion.identity);
            _enemies[i].Pool = this;
            _enemyTransforms.Add(_enemies[i].transform);
            _targetPositions[i] = GetRandomPositionInsideCircle();

            if (!_enemies[i].Movement.Damager.IsNullOrDestroyed()) _playerPositions[i] = _enemies[i].Movement.Damager.position;
            else _playerPositions[i] = _enemies[i].transform.position;

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

            if (!_enemies[i].Movement.Damager.IsNullOrDestroyed()) _playerPositions[i] = _enemies[i].Movement.Damager.position;
            else _playerPositions[i] = _enemies[i].transform.position;

            _isPulling[i] = _enemies[i].Movement.IsPulling;
            _movingSpeed[i] = _enemies[i].IsCaught ? 3 : 1.5f;
        }

        UpdateEnemyMovements();

        for (int i = 0; i < _enemyPool.activeObjects.Count; i++) _enemyPool.activeObjects[i].MonoUpdate();

        SpawnOnCountingEnd();
    }

    private void OnDestroy()
    {
        if (_targetPositions.IsCreated) _targetPositions.Dispose();
        if (_playerPositions.IsCreated) _playerPositions.Dispose();
        if (_enemyTransforms.isCreated) _enemyTransforms.Dispose();
        if (_isPulling.IsCreated) _isPulling.Dispose();
        if (_movingSpeed.IsCreated) _movingSpeed.Dispose();
    }

    public void PullObject() => _enemyPool.PullLocalObject(GetRandomPositionInsideCircle(), Quaternion.identity);

    private float3 GetRandomPositionInsideCircle()
    {
        if (!_enableMove) return new float3(0, 0, 0);

        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);
        float distance = UnityEngine.Random.Range(0f, _boundaryRadius);
        return new float3(math.cos(angle) * distance, 0, math.sin(angle) * distance);
    }

    private void UpdateEnemyMovements()
    {
        var job = new EnemyMovementJob
        {
            EnableMove = _enableMove,
            TargetPositions = _targetPositions,
            PullingSpeed = Player.Movement.CurrentMovingSpeed.Oscillate() * Formula.Value.GetEnemyPullingSpeedMultiplier(transform.position),
            DeltaTime = Time.deltaTime,
            PlayerPositions = _playerPositions,
            IsPulling = _isPulling,
            MovingSpeed = _movingSpeed
        };

        JobHandle handle = job.Schedule(_enemyTransforms);
        handle.Complete();
    }

    public void NotifyOnRespawn()
    {
        _respawnCounter.Add(_enemySources.RespawnTime);

        OnWaitToRespawn?.Invoke(_enemySources.RespawnTime);

        if (_enemyPool.activeObjects.Count <= 0 && _moveWhenClear) this.transform.position = _boundary.GetRandomPositionInRandomBoundary();
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

    public async UniTaskVoid UpdateForSMVirus(int time)
    {
        await UniTask.DelayFrame(2);

        _enemies[0].ReturnToPool();
        if (_respawnCounter.IsEmpty()) _respawnCounter.Add(time);
        else _respawnCounter[0] = time;
    }
}

[BurstCompile]
public struct EnemyMovementJob : IJobParallelForTransform
{
    public bool EnableMove;
    public NativeArray<float3> TargetPositions;
    public float PullingSpeed;
    public float DeltaTime;
    public NativeArray<float3> PlayerPositions;
    public NativeArray<bool> IsPulling;
    public NativeArray<float> MovingSpeed;

    public void Execute(int index, TransformAccess transform)
    {
        if (IsPulling[index])
        {
            transform.position = Vector3.MoveTowards(transform.position, PlayerPositions[index], PullingSpeed);
        }
        else
        {
            if (!EnableMove) return;

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
public class Bound
{
    public Vector3 TopFrontLeft;
    public Vector3 BottomBackRight;
    public bool isEditing;

    public Bound(Vector3 topFrontLeft, Vector3 bottomBackRight, bool isEditing = false)
    {
        TopFrontLeft = topFrontLeft;
        BottomBackRight = bottomBackRight;
        this.isEditing = isEditing;
    }
}
