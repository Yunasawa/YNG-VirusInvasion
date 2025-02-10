using System.Collections.Generic;
using System.Linq;
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
        if (!IsPulling) return;

        List<TentaclePair> groups = Player.Enemy.Tentacles.Where(i => i.Enemy == _manager).ToList();

        foreach (var group in groups)
        {
            if (IsPulling)
            {
                IsPulling = false;
                Enabled = false;
                _manager.IsEnable = false;
                _manager.OnKilled();
                _manager.Stats.OnKilled();

                CameraManager.Instance.Audio.PlayEatingSFX();
            }
            group.Tentacle.RemoveTarget();
            group.Enemy = null;
        }
    }
 }