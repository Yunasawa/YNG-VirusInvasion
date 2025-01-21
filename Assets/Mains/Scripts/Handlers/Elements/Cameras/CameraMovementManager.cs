using DG.Tweening;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;

public class CameraMovementManager : MonoBehaviour
{
    public Transform Target;

    [SerializeField] private float _followingSpeed = 0.05f;

    [SerializeField] private bool _isSmoothMovement = true;

    [SerializeField] private Transform _mainCamera;
    private Vector3 _nearPosition = new(0, 30, -20f);
    private Vector3 _middlePosition = new(0, 40, -25f);
    private Vector3 _farPosition = new(0, 55, -35f);
    private Vector3 _truePosition;
    [SerializeField] private float _focusingSpeed = 0.2f;

    public bool EnableFollowTarget = true;

    private void Awake()
    {
        Player.OnUpgradeAttribute += OnUpgradeAttribute;
        Player.OnRespawn += OnRespawn;
    }

    private void OnDestroy()
    {
        Player.OnUpgradeAttribute -= OnUpgradeAttribute;
        Player.OnRespawn -= OnRespawn;
    }

    private void Start()
    {
        _nearPosition = new(0, 30, -20f);
        _middlePosition = new(0, 40, -25f);
        _farPosition = new(0, 55, -35f);

        OnUpgradeAttribute(AttributeType.Radius);
    }

    private void Update()
    {
        if (EnableFollowTarget)
        {
            FollowTarget();

            if (Player.IsMoving)
            {
                _mainCamera.localPosition = Vector3.Lerp(_mainCamera.localPosition, _truePosition, _focusingSpeed.Oscillate());
            }
            else
            {
                _mainCamera.localPosition = Vector3.Lerp(_mainCamera.localPosition, _nearPosition, _focusingSpeed.Oscillate());
            }
        }
    }

    private void OnUpgradeAttribute(AttributeType type)
    {
        if (type != AttributeType.Radius) return;

        float t = (Formula.Stats.GetRadius() - 15) / 30f;
        _truePosition = Vector3.Lerp(_middlePosition, _farPosition, t);
    }

    private void FollowTarget()
    {
        if (_isSmoothMovement) this.transform.position = Vector3.Lerp(this.transform.position, Target.transform.position, _followingSpeed.Oscillate(60));
        else this.transform.position = Target.transform.position;
    }

    private void OnRespawn()
    {
        this.transform.position = new Vector3(40, 0.5f, 60);
    }
}
