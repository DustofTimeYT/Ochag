using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Контроллер персонажа. Отвечает за действия игрового объекта.
/// Наследует <see cref="EntityController"/>.
/// </summary>
[RequireComponent(typeof(TargetSelector))]
public class PlayerController : EntityController
{
    [Header("Ссылки на модули игрока")]
    [SerializeField] private TargetSelector _targetSelector;
    [SerializeField] private IsometricCameraController _cameraController;

    [Header("Настройки управления игрока")]
    [Tooltip("Скорость плавного разгона и разворота (6–10 — оптимально)")]
    [Range(3f, 25f)][SerializeField] private float _inputSmoothSpeed = 8f;

    [Tooltip("Скорость плавного торможения при отпускании клавиш (15–30 — оптимально)")]
    [Range(5f, 40f)][SerializeField] private float _inputStopSpeed = 20f;

    private bool _jumpPressed = false;
    private bool _walkPressed = false;
    private bool _sprintPressed = false;

    private Vector2 _rawMoveInput;
    private Vector2 _smoothedMoveInput;

    private Transform _cam;

    protected override void Awake()
    {
        base.Awake();
        if (!enabled) return;

        if (!CheckValidRef())
        {
            enabled = false;
            return;
        }

        _cam = _cameraController.transform;
    }

    protected override void Update()
    {
        if (_rawMoveInput.sqrMagnitude < 0.001f)
        {
            // Плавное затухание: даёт естественный микро-буфер при смене W -> S без лишних таймеров
            _smoothedMoveInput = Vector2.MoveTowards(_smoothedMoveInput, Vector2.zero, _inputStopSpeed * Time.deltaTime);
        }
        else
        {
            Vector2 targetDir = _rawMoveInput.normalized;

            if (_smoothedMoveInput.sqrMagnitude < 0.01f || Vector2.Dot(_smoothedMoveInput.normalized, targetDir) < -0.5f)
            {
                // Старт или разворот на 180° — сразу полная скорость
                _smoothedMoveInput = targetDir;
            }
            else
            {
                // Плавный поворот под углом с сохранением полной скорости
                Vector2 next = Vector2.MoveTowards(_smoothedMoveInput, targetDir, _inputSmoothSpeed * Time.deltaTime);
                _smoothedMoveInput = next.sqrMagnitude > 0.001f ? next.normalized : targetDir;
            }
        }


        base.Update();
    }

    // Вспомогательные методы

    /// <summary>
    /// Проверяет наличие всех обязательных ссылок на компоненты.
    /// </summary>
    /// <returns><see langword="true"/>, если все ссылки на компоненты найдены; иначе — <see langword="false"/>.</returns>
    private bool CheckValidRef()
    {
        bool isCameraAssigned = _cameraController != null;
        if (!isCameraAssigned)
        {
            Debug.LogError($"Не назначена ссылка на {nameof(IsometricCameraController)} у {name}!", this);
        }

        return this.CheckComponent(ref _targetSelector) & isCameraAssigned;
    }

    // Получение пользователского ввода
    private void OnMove(InputValue value) => _rawMoveInput = value.Get<Vector2>();
    private void OnJump(InputValue value) { if (value.isPressed) _jumpPressed = true; }
    private void OnWalk(InputValue value) => _walkPressed = value.isPressed;
    private void OnSprint(InputValue value) { if (value.isPressed) _sprintPressed = !_sprintPressed; }

    private void OnLook(InputValue value) => _cameraController.SetLookInput(value.Get<Vector2>());

    private void OnLockTarget(InputValue value)
    {
        if (value.isPressed)
            _targetSelector.ToggleLock();
    }

    private void OnTargetLeft(InputValue value)
    {
        if (value.isPressed)
            _targetSelector.SwitchTarget(-1);
    }

    private void OnTargetRight(InputValue value)
    {
        if (value.isPressed)
            _targetSelector.SwitchTarget(1);
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
        return _targetSelector != null && _targetSelector.HasTarget;
    }

    protected override Vector3? GetTargetPosition()
    {
        if (_targetSelector != null && _targetSelector.HasTarget)
            return _targetSelector.CurrentTarget.AimPosition;

        return null;
    }

    protected override bool HasMoveInput()
    {
        return _rawMoveInput.sqrMagnitude > 0.01f;
    }
}
