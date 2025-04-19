using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CreativeController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _cage;
    [SerializeField] private Transform _cageDoor;
    [SerializeField] private EnemyPool _bossPool;
    [SerializeField] private EmenyStats _stats;
    [SerializeField] private GameObject _effectPanel;
    [SerializeField] private NavMeshAgent _agent;

    [SerializeField] private int _adjustedHeath = 10000;

    private bool _isCageOpened = false;
    private bool _isBossUpdated = false;
    private bool _isEffectReenabled = false;
    private bool _isFollowingPlayer = false;

    private void Start()
    {
        _stats = _bossPool.GetComponentInChildren<EmenyStats>();
        _agent = _stats.AddComponent<NavMeshAgent>();
        _agent.speed = 7;
        _agent.acceleration = 20;
    }

    private void Update()
    {
        if (!_isCageOpened)
        {
            if (Vector3.Distance(_player.position, _cageDoor.position) < 5)
            {
                _isCageOpened = true;

                _cageDoor.DORotate(new(-90, 0, 0), 1);
            }
        }

        if (Game.Data.PlayerStats.CurrentRevolution == 2 && !_isBossUpdated)
        {
            _isBossUpdated = true;

            _stats.CurrentHealth = _adjustedHeath;

            _effectPanel.SetActive(false);
        }

        if (_stats.CurrentHealth <= 0 && !_isEffectReenabled)
        {
            _isEffectReenabled = true;

            _effectPanel.SetActive(true);
        }

        if (Vector3.Distance(_agent.transform.position, _cage.position) > 30 || Vector3.Distance(_agent.transform.position, _player.position) < 5)
        {
            _isFollowingPlayer = false;
        }
        else if (Vector3.Distance(_agent.transform.position, _player.position) < 7)
        {
            _isFollowingPlayer = true;
        }

        if (_isFollowingPlayer)
        {
            _agent.SetDestination(_player.position);
        }
        else
        {
            _agent.SetDestination(_cage.position);
        }
    }
}