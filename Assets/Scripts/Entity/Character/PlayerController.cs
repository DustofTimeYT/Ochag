using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Контроллер персонажа. Отвечает за действия игрового объекта.
/// Наследует <see cref="EntityController"/>.
/// </summary>
public class PlayerController : EntityController
{
    [Tooltip("Чем меньше, тем плавнее разгон и разворот (6–10 — оптимально)")]
    [Range(3f, 25f)][SerializeField] private float _inputSmoothSpeed = 8f;

    private bool _jumpPressed;
    private bool _walkPressed;
    private bool _sprintPressed;

    private Vector2 _rawMoveInput;
    private Vector2 _smoothedMoveInput;

    private Transform _cam;
    protected override void Awake()
    {
        base.Awake();
        _cam = Camera.main.transform;
    }

    // Получение пользователского ввода
    private void OnMove(InputValue value) => _rawMoveInput = value.Get<Vector2>();
    private void OnJump(InputValue value) { if (value.isPressed) _jumpPressed = true; }
    private void OnWalk(InputValue value) => _walkPressed = value.isPressed;
    private void OnSprint(InputValue value) => _sprintPressed = value.isPressed;


    protected override void Update()
    {
        base.Update();
        // Плавно тянем виртуальный стик к целевому значению (работает одинаково хорошо и для WASD, и для геймпада)
        _smoothedMoveInput = Vector2.MoveTowards(_smoothedMoveInput, _rawMoveInput, _inputSmoothSpeed * Time.deltaTime);
    }

    // Переопределение виртуальных и абстрактных методов

    protected override Vector3 GetMoveDirection()
    {
        if (_smoothedMoveInput.sqrMagnitude < 0.001f) return Vector3.zero;

        Vector3 forward = _cam.forward;
        Vector3 right = _cam.right;
        forward.y = 0f;
        right.y = 0f;

        Vector3 rawDir = forward.normalized * _smoothedMoveInput.y + right.normalized * _smoothedMoveInput.x;
        return Vector3.ClampMagnitude(rawDir, 1f);
    }

    protected override bool ConsumeJumpInput()
    {
        if (!_jumpPressed) return false;
        _jumpPressed = false;
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
