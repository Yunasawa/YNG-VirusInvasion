using System.Data;
using UnityEngine;
using YNL.Extensions.Methods;

public class PlayerMovementManager : MonoBehaviour
{
    [SerializeField] private Transform _playerStack;

    private Vector3 _playerStartRotation;
    private float _cameraRotation;
    private Vector2 _axisDirection;

    [SerializeField] private bool _enableRotation = true;

    public float CurrentMovingSpeed = 0.1f;
    [SerializeField] private float _currentRotatingSpeed = 0.5f;

    public bool EnableGravity = false;
    [SerializeField] private float _distance = 0.1f;
    [SerializeField] private float _height = 1;
    [SerializeField] private LayerMask _gravityMask;

    private void Start()
    {
        _playerStartRotation = Player.Transform.rotation.eulerAngles;
    }

    private void Update()
    {
        if (_enableRotation) RotationInputControl(true, true, _currentRotatingSpeed.Oscillate());
        MovementInputControl(CurrentMovingSpeed * 60 * Time.deltaTime);

        ApplyGravity(EnableGravity);

        SyncStackTransform();
    }

    private void ApplyGravity(bool applied)
    {
        if (!applied) return;

        Ray ray = new Ray(Player.Transform.position + Vector3.up * _height, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 10, _gravityMask))
        {
            float higher = Player.Transform.position.y - hit.point.y;
            float lower = hit.point.y - Player.Transform.position.y;

            bool isHigher = higher >= _distance;
            bool isLower = lower >= _distance;

            if (hit.collider.tag == "Water" && isHigher) Player.Character.Move(Vector3.down * higher);
            else if (hit.collider.tag == "Terrain" && isHigher) Player.Character.Move(Vector3.down * higher);
            else if (hit.collider.tag == "Terrain" && isLower) Player.Character.Move(Vector3.up * lower);
        }
    }

    public void MovementInputControl(float speed)
    {
        if (!Game.Manager.IsMobileDevice) _axisDirection = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        else _axisDirection = new(Game.Input.MovementJoystick.Horizontal, Game.Input.MovementJoystick.Vertical);

        if (_axisDirection.magnitude > 0.1f)
        {
            Player.Character.Move(Player.Transform.forward * speed);
            //Player.Animator?.ImmediatePlay("Walk", 0.1f);
        }
        else
        {
            //Player.Animator?.ImmediatePlay("Idle", 0.1f);
        }
    }

    private void RotationInputControl(bool useLocal, bool useInput, float speed)
    {
        if (useInput)
        {
            if (!Game.Manager.IsMobileDevice) _axisDirection = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            else _axisDirection = new(Game.Input.MovementJoystick.Horizontal, Game.Input.MovementJoystick.Vertical);
        }
        if (_axisDirection.magnitude <= 0.1f) return;
        if (useLocal) PlayerRotationEntry(_cameraRotation + Mathf.Atan2(_axisDirection.x, _axisDirection.y) * Mathf.Rad2Deg, speed);
        else PlayerRotationEntry(Mathf.Atan2(_axisDirection.x, _axisDirection.y) * Mathf.Rad2Deg, speed);
    }

    public void PlayerRotationEntry(float degree, float speed)
    {
        if (Player.Manager.IsNull()) return;

        Player.Transform.rotation = Quaternion.Lerp(Player.Transform.rotation, Quaternion.Euler(0, _playerStartRotation.y + degree, 0), speed);
    }

    public void SyncStackTransform()
    {
        _playerStack.position = Player.Transform.position.SetY(0.5f);
        Player.Transform.localPosition = Vector3.zero;
    }
}