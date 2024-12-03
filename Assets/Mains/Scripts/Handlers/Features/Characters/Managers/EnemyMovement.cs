using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Enemy _manager;

    [SerializeField] private float _moveSpeed = 1.5f;
    [SerializeField] private float _changeInterval = 2.0f;
    [SerializeField] private float _range = 5.0f;

    [SerializeField] private Vector3 _targetPosition;
    [SerializeField] private float _directionChangeCounter = 0;

    [HideInInspector] public bool Enabled = true;

    private bool _isPulling = false;
    private float _pullingSpeed;

    private void Start()
    {
        _manager = GetComponent<Enemy>();

        transform.localPosition = GetRandomPosition();
        _targetPosition = GetRandomPosition();
    }

    public void MonoUpdate()
    {
        if (!Enabled) return;

        if (_isPulling)
        {
            _pullingSpeed = Player.Movement.CurrentMovingSpeed * Formula.Value.GetEnemyPullingSpeedMultiplier(transform.position);
            transform.position = Vector3.MoveTowards(transform.position, Player.Transform.position, _pullingSpeed);
            if (Vector3.Distance(transform.position, Player.Transform.position) <= 0.1f) OnPullingDone();
        }
        else
        {
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
    }

    public Vector3 GetRandomPosition()
    {
        Vector3 newPosition;
        int attempts = 0;
        const int maxAttempts = 10;
        float distance;

        do
        {
            newPosition = new Vector3(UnityEngine.Random.Range(-_range, _range), 0, UnityEngine.Random.Range(-_range, _range)) + transform.localPosition;
            distance = Vector3.Distance(transform.localPosition, newPosition);
            attempts++;
        }
        while (Physics.Raycast(transform.position, newPosition - transform.localPosition, out RaycastHit hit, distance, LayerMask.GetMask("Wall")) && attempts < maxAttempts);

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Could not find a valid position within the maximum attempts.");
        }

        return newPosition;
    }


    public void Slowdown(bool enable) => _moveSpeed = enable ? 0.5f : 1.5f;

    public void MoveTowardPlayer()
    {
        if (!_manager.IsCaught) return;

        _isPulling = true;
    }
    public void OnPullingDone()
    {
        _isPulling = false;
        this.gameObject.SetActive(false);
        _manager.Stats.OnKilled();
    }
}