using FIMSpace.FTail;
using UnityEngine;
using YNL.Extensions.Methods;

public class Tentacle : MonoBehaviour
{
    [SerializeField] private Transform _targetPivot;
    public Enemy Target;

    public bool HasTarget = false;

    private void LateUpdate()
    {
        if (HasTarget) _targetPivot.position = Target.transform.position;
    }

    public void SetTarget(Enemy target)
    {
        Target = target;
        HasTarget = true;
        target.OnEnemyKilled = OnEnemyKilled;
    }
    public void RemoveTarget()
    {
        Target = null;
        HasTarget = false;
        _targetPivot.localPosition = new Vector3(0.75f, 0, 0);
    }
    public void OnEnemyKilled()
    {
        RemoveTarget();
    }
}