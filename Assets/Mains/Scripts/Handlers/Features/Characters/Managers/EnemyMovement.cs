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

        MDebug.Log($"Taihen");

        TentaclePair group = Player.Enemy.Tentacles.FirstOrDefault(i => i.Enemy = _manager);
        if (!group.IsNull())
        {
            group.Tentacle.RemoveTarget();
            group.Enemy = null;

            IsPulling = false;
            Enabled = false;
            _manager.IsEnable = false;
            _manager.OnKilled();
            _manager.Stats.OnKilled();

            CameraManager.Instance.Audio.PlayEatingSFX();

            MDebug.Log($"Catch: {_manager.GetInstanceID()}");
        }
    }
}