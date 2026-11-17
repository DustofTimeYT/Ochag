using UnityEngine;

/// <summary>
/// Абстрактная конфигурация для настройки атакующих существ в инспекторе
/// </summary>
public abstract class AttackingEntityConfig : EntityConfig
{
    [SerializeField] private int _armor;
    public int Armor => _armor;

    [SerializeField] private int _strength;
    public int Strength => _strength;

    [SerializeField] private int _agility;
    public int Agility => _agility;

    [SerializeField] private int _maxStamina;
    public int MaxStamina => _maxStamina;
}