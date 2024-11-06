using FIMSpace.FTail;
using UnityEngine;
using YNL.Extensions.Methods;

public class Tentacle : MonoBehaviour
{
    [SerializeField] private Transform _targetPivot;
    public Transform Target;

    [SerializeField] private bool _hasTarget = false;

    private void LateUpdate()
    {
        if (_hasTarget) _targetPivot.position = Target.position;
    }

    public void SetTarget(Transform target)
    {
        Target = target;
        _hasTarget = true;
    }
    public void RemoveTarget()
    {
        _hasTarget = false;
        _targetPivot.localPosition = new Vector3(0.75f, 0, 0);
    }
}