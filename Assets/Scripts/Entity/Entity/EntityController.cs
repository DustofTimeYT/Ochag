using Reflex.Attributes;
using UnityEngine;

/// <summary>
/// Абстрактный контроллер базовых действий существ.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public abstract class EntityController : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    [SerializeField] protected CharacterController _characterController;
    [SerializeField] private float _gravity = -20f;

    [Range(1f, 50f)]
    [SerializeField] private float _rotationSpeed = 10f;

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

    protected IMoveControl MoveControl { get; private set; }

    protected ITickable Tickable { get; private set; }
    private float _verticalVelocity;

    [Inject]
    public void Init(IMoveControl moveControl, ITickable tickable)
    {
        MoveControl = moveControl;
        Tickable = tickable;
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

    private void Update()
    {
        Locomotion();
        Tick();
    }

    private void Tick()
    {
        if (Tickable == null) return;

        Tickable.Tick(Time.deltaTime);
    }

    private void Locomotion()
    {
        if (MoveControl == null) return;

        Move();
        Jump();
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
    /// Отрисовка сферы-чекера в окне Scene, чтобы визуально видеть зону проверки
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (_characterController == null) _characterController = GetComponent<CharacterController>();
        if (_characterController == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(GetGroundCheckPosition(), _checkRadius);
    }

    private void Jump()
    {
        bool isOnGround = GroundCheck();
        // прижимаем к земле энтити
        if (isOnGround && _verticalVelocity < 0f)
        {
            _verticalVelocity = -2f;
        }

        // совершаем прыжок
        if (isOnGround && ConsumeJumpInput())
        {
            _animator.SetTrigger(AnimatorParams.Jump);
            _verticalVelocity = Mathf.Sqrt(MoveControl.MaxJumpHeight * -2f * _gravity);
        }

        _animator.SetBool(AnimatorParams.IsGrounded, isOnGround);

        _verticalVelocity += _gravity * Time.deltaTime;
    }

    private void Move()
    {
        Vector3 dir = GetMoveDirection();

        if (ConsumeSprintingInput() && dir.sqrMagnitude > 0.01f) // Проверяем нажата ли кнопка спринта и движется ли энтити.
        {
            if (MoveControl.TrySprint(Time.deltaTime)) // Проверяем хватает ли стамины.
            {
                _animator.SetBool(AnimatorParams.Sprint, true);
                Move(MoveControl.MaxSprintingSpeed, dir, 1);
                return;
            }
        }
        _animator.SetBool(AnimatorParams.Sprint, false);

        if (ConsumeWalkingInput())
        {
            Move(MoveControl.MaxWalkingSpeed, dir, 0.2f);
        }
        else
        {
            Move(MoveControl.MaxRunningSpeed, dir, 1);
        }
    }

    /// <summary>
    /// Для перемещения и поворота энтити на сцене.
    /// </summary>
    /// <param name="speed">Скорость перемещения энтити.</param>
    /// <param name="stateDir"></param>
    private void Move(float speed, Vector3 dir, float stateDir)
    {
        // поворачиваем энтити по направлению движения
        //Vector3 dir = GetMoveDirection();

        _animator.SetFloat(AnimatorParams.Horizontal, Mathf.Min(Mathf.Abs(dir.z) + Mathf.Abs(dir.x), stateDir));

        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        // перемещаем энтити
        Vector3 movement = dir * speed;
        movement.y = _verticalVelocity;
        _characterController.Move(movement * Time.deltaTime);
    }

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
}
