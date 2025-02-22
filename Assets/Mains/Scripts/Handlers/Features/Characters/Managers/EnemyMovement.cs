using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YNL.Extensions.Methods;

public class EnemyMovement : MonoBehaviour
{
    private Enemy _manager;
    public Transform Damager;

    [HideInInspector] public bool Enabled = true;

    public bool IsPulling = false;

    private void Start()
    {
        _manager = GetComponent<Enemy>();
    }

    [Button]
    public void AAA()
    {
        Damager = this.transform;
    }

    public void MonoUpdate()
    {
        if (!Enabled) return;

        if (!Damager.IsNull())
        {
            if (Vector3.Distance(transform.position, Damager.position) <= 0.1f) OnPullingDone();
        }
    }

    public void SetDamager(Transform damager) => Damager = damager;

    public void MoveTowardPlayer()
    {
        if (!_manager.IsCaught) return;

        IsPulling = true;
    }
    public void OnPullingDone()
    {
        if (!IsPulling) return;

        if (Damager == Player.Transform)
        {
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
        else
        {
            IsPulling = false;
            Enabled = false;
            _manager.IsEnable = false;
            _manager.OnKilled();
            _manager.Stats.OnKilled();
        }

        Damager = transform;
    }
 }