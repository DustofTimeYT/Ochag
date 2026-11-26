using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Контроллер персонажа. Отвечает за действия игрового объекта.
/// Наследует <see cref="EntityController"/>.
/// </summary>
public class PlayerController : EntityController
{
    [Tooltip("Скорость плавного разгона и разворота (6–10 — оптимально)")]
    [Range(3f, 25f)][SerializeField] private float _inputSmoothSpeed = 8f;

    [Tooltip("Скорость плавного торможения при отпускании клавиш (15–30 — оптимально)")]
    [Range(5f, 40f)][SerializeField] private float _inputStopSpeed = 20f;

    private bool _jumpPressed;
    private bool _walkPressed;
    private bool _sprintPressed;
    private bool _isBattleMode;

    private Vector2 _rawMoveInput;
    private Vector2 _smoothedMoveInput;
    private float _releaseBufferTimer;

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
    private void OnLockTarget(InputValue value) { if (value.isPressed) _isBattleMode = !_isBattleMode; }

    protected override void Update()
    {
        if (_rawMoveInput.sqrMagnitude < 0.001f)
        {
            _releaseBufferTimer += Time.deltaTime;
            // Даём 60 мс на перенос пальца между W и S, чтобы не сбрасывать скорость
            if (_releaseBufferTimer > 0.06f)
            {
                _smoothedMoveInput = Vector2.MoveTowards(_smoothedMoveInput, Vector2.zero, _inputSmoothSpeed * 3f * Time.deltaTime);
            }
        }
        else
        {
            _releaseBufferTimer = 0f;

            if (_smoothedMoveInput.sqrMagnitude > 0.01f && Vector2.Dot(_smoothedMoveInput.normalized, _rawMoveInput.normalized) < -0.5f)
            {
                // Резкий разворот на 180° с сохранением текущей скорости
                _smoothedMoveInput = _rawMoveInput.normalized * _smoothedMoveInput.magnitude;
            }
            else
            {
                // Плавный разгон и плавные повороты под углом
                _smoothedMoveInput = Vector2.MoveTowards(_smoothedMoveInput, _rawMoveInput, _inputSmoothSpeed * Time.deltaTime);
            }
        }

        base.Update();
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

    protected override bool IsBattleModeRequested()
    {
        return _isBattleMode;
    }
}
