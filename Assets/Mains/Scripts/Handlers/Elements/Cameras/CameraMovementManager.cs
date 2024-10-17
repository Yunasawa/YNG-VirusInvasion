using Sirenix.OdinInspector;
using UnityEngine;
using YNL.Extensions.Methods;

public class CameraMovementManager : MonoBehaviour
{
    [SerializeField] private Transform _target;

    [SerializeField] private float _followingSpeed = 0.05f;

    [SerializeField] private bool _enableMovement = true;
    [SerializeField] private bool _isLockRotateLimit = true;
    [SerializeField] private bool _isSmoothMovement = true;

    private void Update()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        if (_isSmoothMovement) this.transform.position = Vector3.Lerp(this.transform.position, _target.transform.position, _followingSpeed.Oscillate(60));
        else this.transform.position = _target.transform.position;
    }
}
