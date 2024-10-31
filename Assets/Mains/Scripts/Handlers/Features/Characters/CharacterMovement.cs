using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 1.5f;
    [SerializeField] private float _changeInterval = 2.0f;
    [SerializeField] private float _range = 5.0f;

    [SerializeField] private Vector3 _targetPosition;
    [SerializeField] private float _directionChangeCounter = 0;

    [HideInInspector] public bool Enabled = true;

    private void Start()
    {
        transform.localPosition = GetRandomPosition();
        _targetPosition = GetRandomPosition();
    }

    public void MonoUpdate()
    {
        if (!Enabled) return;

        _directionChangeCounter += Time.deltaTime;

        if (_directionChangeCounter >= _changeInterval)
        {
            _targetPosition = GetRandomPosition();
            _directionChangeCounter = 0.0f;
        }

        Vector3 direction = (_targetPosition - transform.localPosition).normalized;
        if (direction != Vector3.zero)
        { 
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, _moveSpeed * Time.deltaTime);
        }

        transform.localPosition += transform.forward * _moveSpeed * Time.deltaTime;
    }

    public Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-_range, _range), 0, Random.Range(-_range, _range)) + transform.localPosition;
    }
}