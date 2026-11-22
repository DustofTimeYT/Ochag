using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Контроллер персонажа. Отвечает за действия игрового объекта.
/// Наследует <see cref="EntityController"/>.
/// </summary>
public class PlayerController : EntityController
{
    private Vector2 _moveInput;
    private bool _jumpPressed;
    private bool _walkPressed;
    private bool _sprintPressed;

    private Transform _cam;

    private void Awake() => _cam = Camera.main.transform;

    private void OnMove(InputValue value) => _moveInput = value.Get<Vector2>();
    private void OnJump(InputValue value) { if (value.isPressed) _jumpPressed = true; }
    private void OnWalk(InputValue value) => _walkPressed = value.isPressed; //{ if (value.isPressed) _walkPressed = true; }
    private void OnSprint(InputValue value) => _sprintPressed = value.isPressed; //{ if (value.isPressed) _sprintPressed = true; }

    protected override Vector3 GetMoveDirection()
    {
        
        if (_moveInput.sqrMagnitude < 0.01f) return Vector3.zero;

        Vector3 forward = _cam.forward;
        Vector3 right = _cam.right;
        forward.y = 0f;
        right.y = 0f;
        
        return (forward.normalized * _moveInput.y + right.normalized * _moveInput.x).normalized;
    }

    protected override bool ConsumeJumpInput()
    {
        if (!_jumpPressed) return false;
        _jumpPressed = false;
        Debug.Log(_jumpPressed);
        return true;
    }

    protected override bool ConsumeWalkingInput()
    {
        return _walkPressed;
    }

    protected override bool ConsumeSprintingInput()
    {
        return _sprintPressed;
    }
}
