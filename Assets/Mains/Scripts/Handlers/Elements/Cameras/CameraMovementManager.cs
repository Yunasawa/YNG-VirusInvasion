using UnityEngine;
using YNL.Extensions.Methods;

public class CameraMovementManager : MonoBehaviour
{
    public Transform Target;

    [SerializeField] private float _followingSpeed = 0.05f;

    [SerializeField] private bool _isSmoothMovement = true;

    [SerializeField] private Transform _mainCamera;
    private Vector3 _nearPosition = new(0, 30, -16.9f);
    private Vector3 _farPosition = new(0, 50, -26.2f);
    [SerializeField] private float _focusingSpeed = 0.2f;

    private void Update()
    {
        FollowTarget();

        if (Player.IsMoving)
        {
            _mainCamera.localPosition = Vector3.Lerp(_mainCamera.localPosition, _farPosition, _focusingSpeed.Oscillate());
        }
        else
        {
            _mainCamera.localPosition = Vector3.Lerp(_mainCamera.localPosition, _nearPosition, _focusingSpeed.Oscillate());
        }
    }

    private void FollowTarget()
    {
        if (_isSmoothMovement) this.transform.position = Vector3.Lerp(this.transform.position, Target.transform.position, _followingSpeed.Oscillate(60));
        else this.transform.position = Target.transform.position;
    }
}
