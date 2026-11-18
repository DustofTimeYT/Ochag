using UnityEngine;

/// <summary>
/// Абстрактная конфигурация для настройки врагов в инспекторе
/// (<see cref="AttackingEntityConfig"/>).
/// </summary>
public abstract class EnemyConfig : AttackingEntityConfig
{
    [SerializeField] private int _detectionRange;
    public int DetectionRange => _detectionRange;

    [SerializeField] private int _detectionAngleXZ;
    public int DetectionAngleXZ => _detectionAngleXZ;
}