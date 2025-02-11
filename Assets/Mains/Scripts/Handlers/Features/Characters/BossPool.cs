using UnityEngine;
using YNL.Extensions.Methods;

public class BossPool : MonoBehaviour
{
    private EnemyPool _pool;

    private void Awake()
    {
        _pool = GetComponent<EnemyPool>();

        _pool.OnWaitToRespawn += OnWaitToRespawn;
    }

    private void OnDestroy()
    {
        _pool.OnWaitToRespawn -= OnWaitToRespawn;
    }

    private void OnWaitToRespawn(uint time)
    {
        this.gameObject.SetActive(false);
    }
}