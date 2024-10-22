#if false
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public float changeInterval = 2.0f;
    public float range = 5.0f; 

    private Vector3 targetPosition;
    private float timeSinceChange = 0.0f;

    void Start()
    {
        SetRandomTargetPosition();
    }

    void Update()
    {
        timeSinceChange += Time.deltaTime;

        if (timeSinceChange >= changeInterval)
        {
            SetRandomTargetPosition();
            timeSinceChange = 0.0f;
        }

        MoveForward();
    }

    void SetRandomTargetPosition()
    {
        targetPosition = new Vector3(
            Random.Range(-range, range),
            transform.position.y,
            Random.Range(-range, range)
        );
    }

    void MoveForward()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveSpeed * Time.deltaTime);
        }
    }
}
#endif

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

[BurstCompile]
public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public float changeInterval = 2.0f;
    public float range = 5.0f;

    private Vector3 targetPosition;
    private float timeSinceChange = 0.0f;
    private MoveJob moveJob;
    private JobHandle moveJobHandle;

    private TransformAccessArray _accessArray;

    void Start()
    {
        SetRandomTargetPosition();
        _accessArray = new TransformAccessArray(1);
    }

    void Update()
    {
        timeSinceChange += Time.deltaTime;

        if (timeSinceChange >= changeInterval)
        {
            SetRandomTargetPosition();
            timeSinceChange = 0.0f;
        }

        moveJob = new MoveJob
        {
            Position = transform.position,
            Rotation = transform.rotation,
            TargetPosition = targetPosition,
            MoveSpeed = moveSpeed,
            DeltaTime = Time.deltaTime
        };

        JobHandle jobHandle = moveJob.Schedule(_accessArray);
        jobHandle.Complete();
    }

    void SetRandomTargetPosition()
    {
        targetPosition = new Vector3(
            UnityEngine.Random.Range(-range, range),
            transform.position.y,
            UnityEngine.Random.Range(-range, range)
        );
    }
}

[BurstCompile]
public struct MoveJob : IJobParallelForTransform
{
    [ReadOnly] public float3 Position;
    [ReadOnly] public quaternion Rotation;
    [ReadOnly] public float3 TargetPosition;
    [ReadOnly] public float MoveSpeed;
    [ReadOnly] public float DeltaTime;

    public void Execute(int index, TransformAccess transform)
    {
        float3 forward = math.forward(transform.rotation);
        float3 newPosition = (float3)transform.position + forward * MoveSpeed * DeltaTime;
        float3 direction = math.normalize(TargetPosition - newPosition);
        if (!math.all(direction = float3.zero))
        {
            quaternion targetRotation = quaternion.LookRotationSafe(direction, math.up());
            transform.rotation = math.slerp(transform.rotation, targetRotation, MoveSpeed * DeltaTime);
        }

        transform.position = newPosition;
    }
}