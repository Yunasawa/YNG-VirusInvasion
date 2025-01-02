using Cysharp.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using YNL.Extensions.Methods;

public class EnemyMovement : MonoBehaviour
{
    private Enemy _manager;

    [HideInInspector] public bool Enabled = true;

    public bool IsPulling = false;

    private void Start()
    {
        _manager = GetComponent<Enemy>();
    }

    public void MonoUpdate()
    {
        if (!Enabled) return;

        if (Vector3.Distance(transform.position, Player.Transform.position) <= 0.1f) OnPullingDone();
    }

    public void MoveTowardPlayer()
    {
        if (!_manager.IsCaught) return;

        IsPulling = true;
    }
    public void OnPullingDone()
    {
        IsPulling = false;
        _manager.OnKilled();
        _manager.Stats.OnKilled();

        Player.Enemy.Enemies.TryRemove(_manager);

        CameraManager.Instance.Audio.PlayEatingSFX();
    }
}