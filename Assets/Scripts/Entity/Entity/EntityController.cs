using Reflex.Attributes;
using UnityEngine;

/// <summary>
/// Абстрактный контроллер базовых действий существ.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public abstract class EntityController : MonoBehaviour
{
    [SerializeField] protected CharacterController _characterController;
    [SerializeField] private float _gravity = -20f;

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

    private void Start()
    {
        this.CheckComponent(ref _characterController);
    }

    private void FixedUpdate()
    {
        Locomotion();
    }

    private void Update()
    {
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
        // прижимаем к земле энтити
        if (GroundCheck() && _verticalVelocity < 0f)
            _verticalVelocity = -2f;

        // совершаем прыжок
        if (GroundCheck() && ConsumeJumpInput() && MoveControl.TryJump())
            _verticalVelocity = Mathf.Sqrt(MoveControl.MaxJumpHeight * -2f * _gravity);

        _verticalVelocity += _gravity * Time.deltaTime;
    }

    private void Move()
    {
        // поворачиваем энтити по направлению движения
        Vector3 dir = GetMoveDirection();
        if (dir.sqrMagnitude > 0.01f)
            transform.forward = dir;

        // перемещаем энтити
        Vector3 movement = dir * MoveControl.MaxRunningSpeed;
        movement.y = _verticalVelocity;
        _characterController.Move(movement * Time.deltaTime);
    }

    /// <summary>
    /// Получаем направление движения энтити, для его перемещения на сцене
    /// </summary>
    /// <returns>вектор направления движения</returns>
    protected abstract Vector3 GetMoveDirection();

    /// <summary>
    /// Получаем запрос на прыжок
    /// </summary>
    /// <returns>Совершать прыжок?</returns>
    protected virtual bool ConsumeJumpInput() => false;
}
