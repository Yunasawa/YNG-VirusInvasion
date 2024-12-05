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
    [SerializeField] private EnemySources _emenySource;
    [SerializeField] private float _boundaryRadius = 10;
    private int _enemyCount = 0;

    private Enemy[] _enemies;

    private TransformAccessArray _enemyTransforms;
    private NativeArray<float3> _targetPositions;
    private NativeArray<float3> _directions;
    private NativeArray<bool> _isPulling;

    private void Start()
    {
        _enemyCount = _emenySource.EnemyAmount;

        _enemies = new Enemy[_enemyCount];

        _enemyTransforms = new TransformAccessArray(_enemyCount);
        _targetPositions = new NativeArray<float3>(_enemyCount, Allocator.Persistent);
        _directions = new NativeArray<float3>(_enemyCount, Allocator.Persistent);
        _isPulling = new NativeArray<bool>(_enemyCount, Allocator.Persistent);

        for (int i = 0; i < _enemyCount; i++)
        {
            Vector3 spawnPosition = GetRandomPositionInsideCircle();
            _enemies[i] = Instantiate(_emenySource.Enemy, spawnPosition, Quaternion.identity, transform);
            _enemyTransforms.Add(_enemies[i].transform);
            _targetPositions[i] = GetRandomPositionInsideCircle();
            _directions[i] = float3.zero;

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

            _enemies[i].MonoUpdate();
            _isPulling[i] = _enemies[i].Movement.IsPulling;
        }

        UpdateEnemyMovements();
    }

    private void OnDestroy()
    {
        _targetPositions.Dispose();
        _enemyTransforms.Dispose();
        _directions.Dispose();
    }

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
            Directions = _directions,
            PullingSpeed = Player.Movement.CurrentMovingSpeed * Formula.Value.GetEnemyPullingSpeedMultiplier(transform.position),
            MovingSpeed = 0.75f,
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
    public NativeArray<float3> Directions;
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
            if (math.distance(transform.position, PlayerPosition) <= 0.1f)
            {

            }
        }
        else
        {
            float3 position = transform.localPosition;
            float3 direction = math.normalize(TargetPositions[index] - position);
            if (!math.all(direction == float3.zero))
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, MovingSpeed * DeltaTime * 2);

                transform.localPosition += transform.rotation * Vector3.forward * MovingSpeed * DeltaTime;

                Directions[index] = direction;
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
