using UnityEngine;

/// <summary>
/// Абстрактная конфигурация для настройки существ в инспекторе
/// </summary>
public abstract class EntityConfig : ScriptableObject
{
    // _localizationKey будет использоваться для локализации. Если нужно имя то использовать имя файла *.name
    [Header("Системные настройки")]
    [SerializeField] private string _localizationKey;
    public string LocalizationKey => _localizationKey;

    [Header("Настройки передвижения")]
    [Range(0f, 50f)][SerializeField] private float _maxWalkingSpeed = 3f;
    public float MaxWalkingSpeed => _maxWalkingSpeed;

    [Range(0f, 50f)][SerializeField] private float _maxRunningSpeed = 7f;
    public float MaxRunningSpeed => _maxRunningSpeed;

    [Range(0f, 50f)][SerializeField] private float _maxSprintingSpeed = 10f;
    public float MaxSprintingSpeed => _maxSprintingSpeed;

    [Range(0f, 50f)][SerializeField] private float _maxJumpHeight = 2f;
    public float MaxJumpHeight => _maxJumpHeight;

    [Header("Общие характеристики")]
    [Min(1)][SerializeField] private int _maxHP = 150;
    public int MaxHP => _maxHP;

    [Min(1)][SerializeField] private int _maxStamina = 100;
    public int MaxStamina => _maxStamina;

    [Min(0f)][SerializeField] private float _staminaRegen = 10f;
    public float StaminaRegen => _staminaRegen;

    [Min(0f)][SerializeField] private float _sprintStaminaCost = 15f;
    public float SprintStaminaCost => _sprintStaminaCost;

    [Tooltip("Доля стамины (0.2 = 20%), до которой она должна восстановиться после полного истощения.")]
    [Range(0f, 1f)][SerializeField] private float _staminaRecoveryThreshold = 0.2f;
    public float StaminaRecoveryThreshold => _staminaRecoveryThreshold;
}
