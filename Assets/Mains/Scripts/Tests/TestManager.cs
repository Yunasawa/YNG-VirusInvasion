using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;
using TMPro;
using UnityEngine.UI;
using YNL.Extensions.Methods;

public class TestManager : MonoBehaviour
{
    public Button SpawnButton;
    public Button DestroyButton;
    public TMP_InputField InputField;

    public GameObject characterPrefab;
    public int Amount;

    public float moveSpeed = 3.0f;
    public float changeInterval = 2.0f;
    public float range = 5.0f;

    private TransformAccessArray transforms;
    private NativeArray<float3> targetPositions;
    private NativeArray<float> changeIntervals;
    private NativeArray<float> timeSinceChanges;
    private NativeArray<float> moveSpeeds;

    private void Awake()
    {
        SpawnButton.onClick.AddListener(InitializeCharacters);
        DestroyButton.onClick.AddListener(DestroyAll);
        InputField.text = Amount.ToString();
    }

    public void Start()
    {
        InitializeCharacters();
    }

    public void Update()
    {
        for (int i = 0; i < Amount; i++)
        {
            timeSinceChanges[i] += Time.deltaTime;

            if (timeSinceChanges[i] >= changeIntervals[i])
            {
                SetRandomTargetPosition(i);
                timeSinceChanges[i] = 0.0f;
            }
        }

        var moveJob = new MoveJob
        {
            targetPositions = targetPositions,
            moveSpeeds = moveSpeeds,
            deltaTime = Time.deltaTime
        };

        JobHandle moveHandle = moveJob.Schedule(transforms);
        moveHandle.Complete();
    }

    public void OnDestroy()
    {
        transforms.Dispose();
        targetPositions.Dispose();
        changeIntervals.Dispose();
        timeSinceChanges.Dispose();
        moveSpeeds.Dispose();
    }

    public void InitializeCharacters()
    {
        DestroyAll();

        int.TryParse(InputField.text, out Amount);

        transforms = new TransformAccessArray(Amount);
        targetPositions = new NativeArray<float3>(Amount, Allocator.Persistent);
        changeIntervals = new NativeArray<float>(Amount, Allocator.Persistent);
        timeSinceChanges = new NativeArray<float>(Amount, Allocator.Persistent);
        moveSpeeds = new NativeArray<float>(Amount, Allocator.Persistent);

        for (int i = 0; i < Amount; i++)
        {
            GameObject character = Instantiate(characterPrefab, GetRandomPosition(), Quaternion.identity, this.transform);
            transforms.Add(character.transform);

            targetPositions[i] = GetRandomPosition();
            changeIntervals[i] = changeInterval;
            timeSinceChanges[i] = 0.0f;
            moveSpeeds[i] = moveSpeed;
        }
    }

    public void DestroyAll() => this.transform.DestroyAllChildren();

    public void SetRandomTargetPosition(int index)
    {
        targetPositions[index] = GetRandomPosition();
    }

    public Vector3 GetRandomPosition()
    {
        return new Vector3(
            UnityEngine.Random.Range(-range, range),
            0,
            UnityEngine.Random.Range(-range, range)
        );
    }

    [BurstCompile]
    struct MoveJob : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<float3> targetPositions;
        [ReadOnly] public NativeArray<float> moveSpeeds;
        [ReadOnly] public float deltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            float3 position = transform.position;
            float3 direction = math.normalize(targetPositions[index] - position);

            if (!math.all(direction == float3.zero))
            {
                quaternion targetRotation = quaternion.LookRotationSafe(direction, math.up());
                transform.rotation = math.slerp(transform.rotation, targetRotation, moveSpeeds[index] * deltaTime);
            }

            position += math.forward(transform.rotation) * moveSpeeds[index] * deltaTime;
            transform.position = position;
        }
    }
}
