using UnityEngine;
using Unity.Mathematics;

public class TestMovement : MonoBehaviour
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

    public void UpdateMovement()
    {
        timeSinceChange += Time.deltaTime;

        if (timeSinceChange >= changeInterval)
        {
            SetRandomTargetPosition();
            timeSinceChange = 0.0f;
        }
    }

    public void Move()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveSpeed * Time.deltaTime);
        }

        transform.position += transform.forward * moveSpeed * Time.deltaTime;
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
