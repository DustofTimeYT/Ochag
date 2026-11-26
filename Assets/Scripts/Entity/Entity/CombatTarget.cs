using UnityEngine;

/// <summary>
/// Маркер объекта, который может быть захвачен в качестве боевой цели.
/// </summary>
public class CombatTarget : MonoBehaviour
{
    [Tooltip("Точка фокуса (например, центр груди). Если не задана — используется позиция объекта.")]
    [SerializeField] private Transform _aimPoint;

    /// <summary>
    /// Мировая позиция точки прицеливания.
    /// </summary>
    public Vector3 AimPosition => _aimPoint != null ? _aimPoint.position : transform.position;

    /// <summary>
    /// Доступна ли цель для захвата (например, жива и активна).
    /// </summary>
    public bool IsAvailable => isActiveAndEnabled && gameObject.activeInHierarchy;
}
