using FIMSpace.FTail;
using UnityEngine;
using YNL.Extensions.Methods;

public class Tentacle : MonoBehaviour
{
    [SerializeField] private Transform _target;

    [SerializeField] private Transform _targetRig;
    private TailAnimator2 _tailAnimator;

    private void Start()
    {
        _tailAnimator = GetComponent<TailAnimator2>();
    }

    private void Update()
    {
        if (!_target.IsNullOrDestroyed())
        {
            _targetRig.position = _target.position;
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        _tailAnimator.TailAnimatorAmount = 0.5f;
        _tailAnimator.Slithery = 0.3f;
    }
}