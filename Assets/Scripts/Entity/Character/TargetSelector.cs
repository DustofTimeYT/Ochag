using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Постоянно отслеживает доступные боевые цели в радиусе и управляет захватом/переключением фокуса.
/// </summary>
public class TargetSelector : MonoBehaviour
{
    [Header("Настройки поиска")]
    [SerializeField] private LayerMask _targetLayer;

    [Tooltip("Радиус поиска и захвата целей вокруг персонажа (м).")]
    [Range(3f, 30f)][SerializeField] private float _lockRadius = 12f;

    [Tooltip("Половина высоты цилиндра поиска целей по вертикали (м).")]
    [Range(1f, 20f)][SerializeField] private float _lockHeight = 4f;

    [Tooltip("Максимальная дистанция удержания цели; при превышении захват автоматически сбрасывается (м).")]
    [Range(5f, 40f)][SerializeField] private float _breakDistance = 16f;

    [Tooltip("Интервал обновления списка целей вокруг персонажа (сек).")]
    [Range(0.02f, 0.5f)][SerializeField] private float _scanInterval = 0.1f;

    private readonly Collider[] _hitBuffer = new Collider[24];
    private readonly List<CombatTarget> _candidates = new List<CombatTarget>(24);
    private float _scanTimer;

    public CombatTarget CurrentTarget { get; private set; }
    public bool HasTarget => CurrentTarget != null && CurrentTarget.IsAvailable;
    public bool HasAnyTargetNearby => _candidates.Count > 0;

    private void Update()
    {
        // Авто-сброс текущей цели, если она недоступна или ушла за дистанцию разрыва
        if (CurrentTarget != null)
        {
            Vector3 flatOffset = CurrentTarget.AimPosition - transform.position;
            flatOffset.y = 0f;
            if (!CurrentTarget.IsAvailable || flatOffset.sqrMagnitude > _breakDistance * _breakDistance)
            {
                ClearTarget();
            }
        }

        // Периодически обновляем буфер целей вокруг без выделения памяти
        _scanTimer += Time.deltaTime;
        if (_scanTimer >= _scanInterval)
        {
            _scanTimer = 0f;
            ScanTargets();
        }
    }

    /// <summary>
    /// Захватывает ближайшую цель из актуального списка или сбрасывает текущий захват (кнопка G).
    /// </summary>
    public void ToggleLock()
    {
        if (HasTarget)
        {
            ClearTarget();
            return;
        }

        ScanTargets();
        CurrentTarget = GetNearestFromCandidates();
    }

    /// <summary>
    /// Циклически переключает цель по кругу влево (-1, кнопка R) или вправо (+1, кнопка T).
    /// </summary>
    public void SwitchTarget(int directionSign)
    {
        ScanTargets();

        if (!HasTarget) return;

        if (_candidates.Count <= 1) return;

        int currentIndex = _candidates.IndexOf(CurrentTarget);
        if (currentIndex < 0)
        {
            CurrentTarget = GetNearestFromCandidates();
            return;
        }

        int nextIndex = (currentIndex + directionSign + _candidates.Count) % _candidates.Count;
        CurrentTarget = _candidates[nextIndex];
    }

    public void ClearTarget()
    {
        CurrentTarget = null;
    }

    private void ScanTargets()
    {
        _candidates.Clear();

        Vector3 center = transform.position;
        Vector3 pointBottom = center - Vector3.up * _lockHeight;
        Vector3 pointTop = center + Vector3.up * _lockHeight;

        int count = Physics.OverlapCapsuleNonAlloc(pointBottom, pointTop, _lockRadius, _hitBuffer, _targetLayer, QueryTriggerInteraction.Ignore);
        float maxSqrRadius = _lockRadius * _lockRadius;

        for (int i = 0; i < count; i++)
        {
            CombatTarget candidate = ResolveTarget(_hitBuffer[i]);
            if (candidate == null || !candidate.IsAvailable || _candidates.Contains(candidate))
                continue;

            // Проверяем горизонтальное расстояние (XZ), чтобы получить строгий цилиндр, а не капсулу
            Vector3 flatOffset = candidate.AimPosition - center;
            flatOffset.y = 0f;
            if (flatOffset.sqrMagnitude > maxSqrRadius)
                continue;

            _candidates.Add(candidate);
        }

        _candidates.Sort((a, b) =>
        {
            Vector3 dirA = a.AimPosition - center;
            Vector3 dirB = b.AimPosition - center;
            return Mathf.Atan2(dirA.x, dirA.z).CompareTo(Mathf.Atan2(dirB.x, dirB.z));
        });
    }

    private CombatTarget GetNearestFromCandidates()
    {
        CombatTarget nearest = null;
        float bestSqrDist = float.MaxValue;

        for (int i = 0; i < _candidates.Count; i++)
        {
            float sqrDist = (_candidates[i].AimPosition - transform.position).sqrMagnitude;
            if (sqrDist < bestSqrDist)
            {
                bestSqrDist = sqrDist;
                nearest = _candidates[i];
            }
        }

        return nearest;
    }

    private static CombatTarget ResolveTarget(Collider col)
    {
        if (col.TryGetComponent(out CombatTarget target))
            return target;

        return col.GetComponentInParent<CombatTarget>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 center = transform.position;
        Vector3 topCenter = center + Vector3.up * _lockHeight;
        Vector3 bottomCenter = center - Vector3.up * _lockHeight;

        const int segments = 32;
        float step = Mathf.PI * 2f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angleA = i * step;
            float angleB = (i + 1) * step;

            Vector3 offsetA = new Vector3(Mathf.Cos(angleA) * _lockRadius, 0f, Mathf.Sin(angleA) * _lockRadius);
            Vector3 offsetB = new Vector3(Mathf.Cos(angleB) * _lockRadius, 0f, Mathf.Sin(angleB) * _lockRadius);

            Gizmos.DrawLine(topCenter + offsetA, topCenter + offsetB);
            Gizmos.DrawLine(bottomCenter + offsetA, bottomCenter + offsetB);
        }

        // 4 вертикальные линии, соединяющие верхнее и нижнее основания цилиндра
        Gizmos.DrawLine(topCenter + Vector3.forward * _lockRadius, bottomCenter + Vector3.forward * _lockRadius);
        Gizmos.DrawLine(topCenter - Vector3.forward * _lockRadius, bottomCenter - Vector3.forward * _lockRadius);
        Gizmos.DrawLine(topCenter + Vector3.right * _lockRadius, bottomCenter + Vector3.right * _lockRadius);
        Gizmos.DrawLine(topCenter - Vector3.right * _lockRadius, bottomCenter - Vector3.right * _lockRadius);
    }
}
