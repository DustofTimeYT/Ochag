using Reflex.Attributes;
using System.Collections;
using UnityEngine;

/// <summary>
/// Абстрактный контроллер базовых действий существ.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public abstract class EntityController : MonoBehaviour
{
    [Header("Ссылки на контроллеры")]
    [SerializeField] protected Animator _animator;
    [SerializeField] protected CharacterController _characterController;
    protected IMoveControl MoveControl { get; private set; }
    protected ITickable Tickable { get; private set; }
    protected IBattleModeControl BattleModeControl { get; private set; }

    [Header("Настройки физики")]
    [SerializeField] private float _gravity = -20f;

    [Header("Настройки управления")]
    [Range(1f, 50f)] [SerializeField] private float _rotationSpeed = 10f;
    private float _verticalVelocity;
    private bool _isPreparingJump;

    [Header("Настройки управления в режиме битвы")]

    private bool _isBattleMode;

    [Header("Настройки проверки земли")]

    /// <summary>
    /// Выбор слоев, на которых энтити может прыгать
    /// </summary>
    [SerializeField] private LayerMask _groundLayer = ~0;

    /// <summary>
    /// Корректировка радиуса сферы для проверки нахождения (сферы-чекера) энтити на земле
    /// </summary>
    [SerializeField] private float _checkRadius = 0.25f;

    /// <summary>
    /// Корректировка сферы-чекера энтити на земле по высоте
    /// </summary>
    [Range(-0.5f, 0.5f)]
    [SerializeField] private float _groundCheckOffset = 0.05f;
    private bool _isRawGrounded;

    [Inject]
    public void Init(IMoveControl moveControl, ITickable tickable, IBattleModeControl battleModeControl)
    {
        MoveControl = moveControl;
        Tickable = tickable;
        BattleModeControl = battleModeControl;
    }

    /// <summary>
    /// Инициализирует и валидирует обязательные ссылки на компоненты перед началом работы контроллера.
    /// При отсутствии критических компонентов отключает выполнение скрипта.
    /// </summary>
    protected virtual void Awake()
    {
        if (!CheckValidRef())
        {
            enabled = false;
            return;
        }
    }

    protected virtual void Update()
    {
        Locomotion();
        Tick();
    }

    // Вспомогательные методы

    /// <summary>
    /// Проверяет наличие всех обязательных компонентов на текущем или дочерних объектах.
    /// </summary>
    /// <returns><see langword="true"/>, если все компоненты найдены; иначе — <see langword="false"/>.</returns>
    private bool CheckValidRef()
    {
        return
            this.CheckComponentInChildren(ref _animator) &
            this.CheckComponent(ref _characterController);
    }

    private void Tick()
    {
        if (Tickable == null) return;

        Tickable.Tick(Time.deltaTime);
    }

    private void Locomotion()
    {
        if (MoveControl == null) return;

        UpdateBattleMode();

        Move();
        Jump();
    }

    // Режим боя

    private void UpdateBattleMode()
    {
        bool requested = IsBattleModeRequested();
        if (_isBattleMode == requested) return;

        _isBattleMode = requested;
        _animator.SetBool(AnimatorParams.BattleMode, _isBattleMode);
        BattleModeControl?.SetBattleMode(_isBattleMode);
    }

    // Проверка земли

    /// <summary>
    /// Проверяет стоит ли энтити на земле
    /// </summary>
    /// <returns>находиться ли на земле</returns>
    private bool GroundCheck()
    {
        return Physics.CheckSphere(
            GetGroundCheckPosition(),
            _checkRadius,
            _groundLayer,
            QueryTriggerInteraction.Ignore
        );
    }

    /// <summary>
    /// Находит виртуальное положения сферы-чекера для проверки земли 
    /// </summary>
    /// <returns>Положение виртуальной сферы-чекера</returns>
    private Vector3 GetGroundCheckPosition()
    {
        float bottomY = _characterController.center.y - (_characterController.height * 0.5f) + _checkRadius - _groundCheckOffset;
        return transform.position + new Vector3(_characterController.center.x, bottomY, _characterController.center.z);
    }

    /// <summary>
    /// Отрисовка сферы-чекера в окне Scene, чтобы визуально видеть зону проверки
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (_characterController == null) _characterController = GetComponent<CharacterController>();
        if (_characterController == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(GetGroundCheckPosition(), _checkRadius);
    }

    // Обработка прыжка

    private void Jump()
    {
        _isRawGrounded = GroundCheck();

        // Прижимаем к земле энтити
        if (_isRawGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = -2f;
        }

        // 2. Разрешаем старт прыжка
        if (_isRawGrounded && ConsumeJumpInput() && !_isPreparingJump)
        {
            _isPreparingJump = true;
            _animator.SetTrigger(AnimatorParams.Jump);
        }

        _animator.SetBool(AnimatorParams.IsGrounded, _isRawGrounded);
        _verticalVelocity += _gravity * Time.deltaTime;
    }

    /// <summary>
    /// Вызывается через Animation Event в кадре отрыва ног от земли.
    /// </summary>
    public void OnJumpTakeoff()
    {
        if (!_isPreparingJump) return;

        _isPreparingJump = false;
        _verticalVelocity = Mathf.Sqrt(MoveControl.MaxJumpHeight * -2f * _gravity);
    }

    // Обработка движения

    /// <summary>
    /// Определение вида (шаг/бег/спринт) движения энтити
    /// </summary>
    private void Move()
    {
        Vector3 dir = GetMoveDirection();

        if (_isPreparingJump) return;

        if (ConsumeSprintingInput() && dir.sqrMagnitude > 0.01f)
        {
            if (MoveControl.TrySprint(Time.deltaTime))
            {
                _animator.SetBool(AnimatorParams.Sprint, true);
                Move(MoveControl.MaxSprintingSpeed, dir, 1f, true);
                return;
            }
        }
        _animator.SetBool(AnimatorParams.Sprint, false);

        if (ConsumeWalkingInput())
        {
            Move(MoveControl.MaxWalkingSpeed, dir, 0.2f, false);
        }
        else
        {
            Move(MoveControl.MaxRunningSpeed, dir, 1f, false);
        }
    }

    /// <summary>
    /// Для перемещения и поворота энтити на сцене.
    /// </summary>
    /// <param name="speed">Скорость перемещения энтити.</param>
    /// <param name="dir">Направление движения в мировых координатах.</param>
    /// <param name="stateDir">Множитель анимации скорости (0.2 для шага, 1 для бега).</param>
    /// <param name="isSprinting">Активен ли в данный момент спринт.</param>
    private void Move(float speed, Vector3 dir, float stateDir, bool isSprinting)
    {
        Vector3? targetPos = GetTargetPosition();

        if (dir.magnitude < 0.08f)
        {
            dir = Vector3.zero;
        }

        // Стрейфы и фиксация взгляда включаются только в боевом режиме, при наличии цели и без спринта
        if (_isBattleMode && targetPos.HasValue && !isSprinting)
        {
            if (targetPos.HasValue)
            {
                Vector3 toTarget = targetPos.Value - transform.position;
                toTarget.y = 0f;
                if (toTarget.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(toTarget);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
                }
            }

            Vector3 localDir = transform.InverseTransformDirection(dir) * stateDir;
            _animator.SetFloat(AnimatorParams.Horizontal, localDir.x);
            _animator.SetFloat(AnimatorParams.Vertical, localDir.z);
        }
        else
        {
            // Вне боя или во время спринта поворачиваемся лицом по направлению движения
            float animSpeed = isSprinting ? 1f : dir.magnitude * stateDir;
            _animator.SetFloat(AnimatorParams.Horizontal, animSpeed);
            _animator.SetFloat(AnimatorParams.Vertical, 0f);

            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }

        // Перемещаем энтити
        Vector3 movement = dir * speed;
        movement.y = _verticalVelocity;
        _characterController.Move(movement * Time.deltaTime);
    }

    // Виртуальные и абстрактные методы

    /// <summary>
    /// Получаем направление движения, для перемещения на сцене.
    /// </summary>
    /// <returns><see cref="Vector3"/>, вектор направления движения в диапазоне [-1;1].</returns>
    protected abstract Vector3 GetMoveDirection();

    /// <summary>
    /// Подтверждание прыжка от класса-наследника.
    /// </summary>
    /// <returns><see langword="true"/>, если нужно совершить прыжок; иначе — <see langword="false"/>.</returns>
    protected virtual bool ConsumeJumpInput() => false;

    /// <summary>
    /// Подтверждание перехода на шаг от класса-наследника.
    /// </summary>
    /// <returns><see langword="true"/>, если нужно переключиться на шаг; иначе — <see langword="false"/>.</returns>
    protected virtual bool ConsumeWalkingInput() => false;

    /// <summary>
    /// Подтверждание перехода на спринт от класса-наследника.
    /// </summary>
    /// <returns><see langword="true"/>, если нужно переключиться на спринт; иначе — <see langword="false"/>.</returns>
    protected virtual bool ConsumeSprintingInput() => false;

    /// <summary>
    /// Определяет, должен ли персонаж находиться в боевом режиме.
    /// </summary>
    protected virtual bool IsBattleModeRequested() => false;

    /// <summary>
    /// Возвращает мировую позицию текущей цели в боевом режиме (если цель захвачена).
    /// </summary>
    protected virtual Vector3? GetTargetPosition() => null;
}
