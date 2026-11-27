using UnityEngine;

/// <summary>
/// Контроллер изометрической камеры: плавно следует за целью, смещается при захвате противника
/// и позволяет игроку осматриваться вокруг (правый стик / стрелочки).
/// </summary>
public class IsometricCameraController : MonoBehaviour
{
    [Header("Цель слежения")]
    [Tooltip("Трансформ персонажа, за которым следует камера.")]
    [SerializeField] private Transform _followTarget;

    [Tooltip("Опциональный селектор целей для смещения камеры в сторону захваченного врага.")]
    [SerializeField] private TargetSelector _targetSelector;

    [Header("Позиция и плавность")]
    [Tooltip("Базовое смещение камеры относительно персонажа (настраивается автоматически при старте, если оставить нулём).")]
    [SerializeField] private Vector3 _baseOffset;

    [Tooltip("Время сглаживания движения камеры (меньше — резче, больше — плавнее).")]
    [Range(0.02f, 0.5f)][SerializeField] private float _smoothTime = 0.15f;

    [Header("Осмотр и боевой фокус")]
    [Tooltip("Максимальная дистанция ручного смещения камеры стрелочками / правым стиком (м).")]
    [Range(0f, 10f)][SerializeField] private float _lookOffsetDistance = 3.5f;

    [Tooltip("Скорость реакции камеры на ручной ввод осмотра.")]
    [Range(1f, 20f)][SerializeField] private float _lookSmoothSpeed = 8f;

    [Tooltip("Доля смещения камеры в сторону захваченного врага (0 — строго на игроке, 0.5 — ровно посередине).")]
    [Range(0f, 0.5f)][SerializeField] private float _targetFocusWeight = 0.25f;

    private Vector2 _rawLookInput;
    private Vector3 _currentLookOffset;
    private Vector3 _currentVelocity;

    private void Awake()
    {
        if (!CheckValidRef())
        {
            enabled = false;
            return;
        }

        // Если смещение не задано вручную в инспекторе — запоминаем текущую расстановку из сцены
        if (_baseOffset == Vector3.zero)
        {
            _baseOffset = transform.position - _followTarget.position;
        }
    }

    private void LateUpdate()
    {
        if (_followTarget == null) return;

        Vector3 focusPoint;
        Vector3 desiredLookOffset;

        if (_targetSelector != null && _targetSelector.HasTarget)
        {
            // В бою с захваченной целью смещаем фокус между игроком и врагом, отключая ручной сдвиг
            Vector3 targetPos = _targetSelector.CurrentTarget.AimPosition;
            targetPos.y = _followTarget.position.y;
            focusPoint = Vector3.Lerp(_followTarget.position, targetPos, _targetFocusWeight);
            desiredLookOffset = Vector3.zero;
        }
        else
        {
            // Вне захвата цели фокус строго на игроке + свободный обзор стрелочками / правым стиком
            focusPoint = _followTarget.position;
            desiredLookOffset = CalculateManualLookOffset();
        }

        _currentLookOffset = Vector3.Lerp(_currentLookOffset, desiredLookOffset, _lookSmoothSpeed * Time.deltaTime);

        // 3. Плавно двигаем камеру в целевую позицию через SmoothDamp
        Vector3 desiredPosition = focusPoint + _baseOffset + _currentLookOffset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, _smoothTime);
    }

    // Вспомогательные методы

    /// <summary>
    /// Проверяет наличие всех обязательных ссылок на компоненты.
    /// </summary>
    /// <returns><see langword="true"/>, если все ссылки на компоненты найдены; иначе — <see langword="false"/>.</returns>
    private bool CheckValidRef()
    {
        bool isValid = true;

        if (_followTarget == null)
        {
            Debug.LogError($"Не назначена цель слежения (_followTarget) на камере {name}!", this);
            isValid = false;
        }

        if (_targetSelector == null)
        {
            Debug.LogError($"Не назначен селектор целей (_targetSelector) на камере {name}!", this);
            isValid = false;
        }

        return isValid;
    }

    /// <summary>
    /// Устанавливает вектор направления осмотра (от правого стика или стрелочек).
    /// </summary>
    public void SetLookInput(Vector2 lookInput)
    {
        _rawLookInput = lookInput;
    }

    private Vector3 CalculateManualLookOffset()
    {
        if (_rawLookInput.sqrMagnitude < 0.001f)
            return Vector3.zero;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0f;
        right.y = 0f;

        Vector3 worldDir = forward.normalized * _rawLookInput.y + right.normalized * _rawLookInput.x;
        return Vector3.ClampMagnitude(worldDir, 1f) * _lookOffsetDistance;
    }
}