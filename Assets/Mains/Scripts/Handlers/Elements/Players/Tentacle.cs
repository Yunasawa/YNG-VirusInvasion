using FIMSpace.FTail;
using UnityEngine;
using YNL.Extensions.Methods;

public class Tentacle : MonoBehaviour
{
    public Transform Target;

    public Transform TargetRig;
    private TailAnimator2 _tailAnimator;

    [SerializeField] private bool _hasTarget = false;

    private void Start()
    {
        _tailAnimator = GetComponent<TailAnimator2>();
    }

    private void Update()
    {
        if (_hasTarget) TargetRig.position = Target.position;
    }

    public void SetTarget(Transform target)
    {
        Target = target;
        _hasTarget = true;
        _tailAnimator.TailAnimatorAmount = 0.5f;
        _tailAnimator.Slithery = 0.3f;
    }
    public void RemoveTarget()
    {
        Target = null;
        _hasTarget = false;
        _tailAnimator.TailAnimatorAmount = 1;
        _tailAnimator.Slithery = 1;
    }
}